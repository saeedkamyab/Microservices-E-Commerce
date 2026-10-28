using Catalog.Domain.Events;
using SharedKernel.Domain;
using System.Text.Json;

namespace Catalog.Infrastructure.Persistence.Outbox;

public static class OutboxMessageFactory
{
    public static OutboxMessage? Create(
        IDomainEvent domainEvent)
    {
        return domainEvent switch
        {
            ProductActivatedDomainEvent e => new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = "ProductActivated",
                Payload = JsonSerializer.Serialize(new
                {
                    e.ProductId,
                    e.OccurredOnUtc
                }),
                CreatedAtUtc = DateTime.UtcNow
            },

            _ => null
        };
    }
}