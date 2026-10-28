using Catalog.Application.Abstractions.Persistence;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Persistence.Models;
using Catalog.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence;

public class CatalogDbContext:DbContext, IUnitOfWork
{
    public CatalogDbContext(
        DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }
    public DbSet<Category> Categories => Set<Category>();

    internal DbSet<CategoryAttributeDefinitionRecord>
   CategoryAttributeDefinitions =>
       Set<CategoryAttributeDefinitionRecord>();

    internal DbSet<AttributeOptionRecord>
        AttributeOptions =>
            Set<AttributeOptionRecord>();

    public DbSet<Product> Products => Set<Product>();

    internal DbSet<ProductSpecificationRecord> ProductSpecifications =>
   Set<ProductSpecificationRecord>();

    internal DbSet<OutboxMessage> OutboxMessages =>
    Set<OutboxMessage>();

    protected override void OnModelCreating(
       ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(CatalogDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(
    CancellationToken cancellationToken = default)
    {
        var productsWithDomainEvents = ChangeTracker
            .Entries<Product>()
            .Select(entry => entry.Entity)
            .Where(product => product.DomainEvents.Count > 0)
            .ToList();

        var domainEvents = productsWithDomainEvents
            .SelectMany(product => product.DomainEvents)
            .ToList();

        var outboxMessages = domainEvents
            .Select(OutboxMessageFactory.Create)
            .Where(message => message is not null)
            .Select(message => message!)
            .ToList();

        if (outboxMessages.Count > 0)
        {
            await OutboxMessages.AddRangeAsync(
                outboxMessages,
                cancellationToken);
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var product in productsWithDomainEvents)
        {
            product.ClearDomainEvents();
        }

        return result;
    }
}
