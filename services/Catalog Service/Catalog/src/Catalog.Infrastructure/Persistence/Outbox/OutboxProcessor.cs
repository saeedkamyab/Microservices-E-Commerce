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

        // Each message is processed in its own transaction.
        // SKIP LOCKED prevents multiple workers from
        // processing the same message concurrently.

        for (var i = 0; i < 20; i++)
        {
            await using var transaction =
                await dbContext.Database.BeginTransactionAsync(
                    cancellationToken);

            var message = await dbContext.OutboxMessages
                .FromSqlRaw("""
                SELECT *
                FROM "OutboxMessages"
                WHERE "ProcessedAtUtc" IS NULL
                  AND "Type" = 'CacheInvalidation'
                ORDER BY "CreatedAtUtc", "Id"
                LIMIT 1
                FOR UPDATE SKIP LOCKED
                """)
                .AsTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (message is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                break;
            }

            try
            {
                using var document =
                    JsonDocument.Parse(message.Payload);

                var key = document.RootElement
                    .GetProperty("CacheVersionKey")
                    .GetString();

                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new InvalidOperationException(
                        "CacheVersionKey is missing.");
                }

                await versionService.IncrementVersionAsync(
                    key,
                    cancellationToken);

                message.ProcessedAtUtc = DateTime.UtcNow;
                message.LastError = null;

                await dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

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
                await transaction.RollbackAsync(
                    cancellationToken);

                // Discard tracked state after rollback.
                dbContext.ChangeTracker.Clear();

                message.RetryCount++;
                message.LastError = ex.Message.Length > 2000
                    ? ex.Message[..2000]
                    : ex.Message;

                try
                {
                    // Persist failure information separately.
                    // The message remains available for retry.
                    dbContext.OutboxMessages.Attach(message);

                    dbContext.Entry(message)
                        .Property(x => x.RetryCount)
                        .IsModified = true;

                    dbContext.Entry(message)
                        .Property(x => x.LastError)
                        .IsModified = true;

                    await dbContext.SaveChangesAsync(
                        cancellationToken);
                }
                catch (Exception saveException)
                {
                    logger.LogError(
                        saveException,
                        "Failed to persist outbox error.");

                    dbContext.ChangeTracker.Clear();
                }

                logger.LogWarning(
                    ex,
                    "Outbox message {MessageId} failed.",
                    message.Id);

                // Avoid immediately retrying the same failed
                // message repeatedly in this processing cycle.
                break;
            }
        }
    }


}