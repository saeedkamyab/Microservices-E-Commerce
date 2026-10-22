using Catalog.Application.Abstractions.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Catalog.Infrastructure.Persistence.Outbox;

public sealed class OutboxProcessor(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxProcessor> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(
            TimeSpan.FromSeconds(5));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessMessagesAsync(stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Outbox processing failed.");
            }

            if (!await timer.WaitForNextTickAsync(stoppingToken))
                break;
        }
    }

    private async Task ProcessMessagesAsync(
        CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<CatalogDbContext>();

        var versionService = scope.ServiceProvider
            .GetRequiredService<ICacheVersionService>();

        var messages = await dbContext.OutboxMessages
            .Where(x =>
                x.ProcessedAtUtc == null &&
                x.Type == "CacheInvalidation")
            .OrderBy(x => x.CreatedAtUtc)
            .Take(20)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                using var document =
                    JsonDocument.Parse(message.Payload);

                var key = document.RootElement
                    .GetProperty("CacheVersionKey")
                    .GetString();

                if (string.IsNullOrWhiteSpace(key))
                    throw new InvalidOperationException(
                        "CacheVersionKey is missing.");

                await versionService.IncrementVersionAsync(
                    key, cancellationToken);

                message.ProcessedAtUtc = DateTime.UtcNow;
                message.LastError = null;

                await dbContext.SaveChangesAsync(
                    cancellationToken);

                logger.LogInformation(
                    "Outbox message {MessageId} processed.",
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
                message.LastError = ex.Message.Length > 2000
                    ? ex.Message[..2000]
                    : ex.Message;

                try
                {
                    await dbContext.SaveChangesAsync(
                        cancellationToken);
                }
                catch (Exception saveException)
                {
                    logger.LogError(
                        saveException,
                        "Failed to persist outbox error.");
                }

                logger.LogWarning(
                    ex,
                    "Outbox message {MessageId} failed.",
                    message.Id);
            }
        }
    }
}