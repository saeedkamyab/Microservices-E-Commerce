using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;


namespace Catalog.Infrastructure.Messaging;

public sealed class ProductActivatedPublisher : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqPublisher _publisher;
    private readonly ILogger<ProductActivatedPublisher> _logger;

    public ProductActivatedPublisher(
        IServiceScopeFactory scopeFactory,
        RabbitMqPublisher publisher,
        ILogger<ProductActivatedPublisher> logger)
    {
        _scopeFactory = scopeFactory;
        _publisher = publisher;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "ProductActivatedPublisher started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingMessagesAsync(stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing ProductActivated outbox messages.");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(5),
                stoppingToken);
        }
    }

    private async Task ProcessPendingMessagesAsync(
        CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<CatalogDbContext>();

        var messages = await dbContext.Set<OutboxMessage>()
            .Where(message =>
                message.Type == "ProductActivated" &&
                message.ProcessedAtUtc == null &&
                message.FailedAtUtc == null &&
                (message.NextRetryAtUtc == null ||
                 message.NextRetryAtUtc <= DateTime.UtcNow))
            .OrderBy(message => message.CreatedAtUtc)
            .Take(20)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(message.Payload);

                await _publisher.PublishAsync(
                    CatalogRoutingKeys.ProductActivated,
                    body,
                    cancellationToken);

                message.ProcessedAtUtc = DateTime.UtcNow;
                message.LastError = null;
                message.NextRetryAtUtc = null;

                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Published ProductActivated outbox message {MessageId}.",
                    message.Id);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                message.RetryCount++;
                message.LastError = ex.Message;

                if (message.RetryCount >= 5)
                {
                    message.FailedAtUtc = DateTime.UtcNow;
                    message.NextRetryAtUtc = null;
                }
                else
                {
                    var delaySeconds = Math.Min(
                        60,
                        Math.Pow(2, message.RetryCount));

                    message.NextRetryAtUtc =
                        DateTime.UtcNow.AddSeconds(delaySeconds);
                }

                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogError(
                    ex,
                    "Failed to publish outbox message {MessageId}. Retry {RetryCount}.",
                    message.Id,
                    message.RetryCount);
            }
        }
    }
}