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


}
