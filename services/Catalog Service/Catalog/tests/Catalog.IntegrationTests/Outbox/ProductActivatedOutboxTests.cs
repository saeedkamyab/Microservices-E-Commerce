using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.ValueObjects;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence.Outbox;
using Catalog.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Catalog.IntegrationTests.Outbox;

public sealed class ProductActivatedOutboxTests(
   PostgreSqlFixture fixture)
   : IClassFixture<PostgreSqlFixture>
{
    [Fact]
    public async Task SaveChangesAsync_WhenProductIsActivated_PersistsOutboxMessageAndClearsDomainEvents()
    {
        // Arrange
        await using var scope = fixture.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<CatalogDbContext>();

        var category = Category.Create(CategoryName.Create("Electronics"));
        dbContext.Categories.Add(category);

        var product = Product.Create(
            ProductName.Create($"Test Product {Guid.NewGuid():N}"),
            "Integration test product",
            category.Id,
            Price.Create(100));

        product.Activate();

        dbContext.Products.Add(product);

        // Act
        await dbContext.SaveChangesAsync(CancellationToken.None);

        Assert.Empty(product.DomainEvents);

        // Assert: Product and Outbox message were persisted.
        var persistedProduct = await dbContext.Products
            .AsNoTracking()
            .SingleAsync(x => x.Id == product.Id,CancellationToken.None);

        Assert.Equal(ProductStatus.Active, persistedProduct.Status);

        var outboxMessages = await dbContext.Set<OutboxMessage>()
     .Where(x => x.Type == "ProductActivated")
     .ToListAsync(CancellationToken.None);

        var outboxMessage = Assert.Single(outboxMessages);

        using var payload = JsonDocument.Parse(outboxMessage.Payload);

        Assert.Equal(
            product.Id,
            payload.RootElement
                .GetProperty("ProductId")
                .GetGuid());

        Assert.NotEqual(Guid.Empty, outboxMessage.Id);
        Assert.NotEqual(default, outboxMessage.CreatedAtUtc);

       

        Assert.Equal(
            product.Id,
            payload.RootElement
                .GetProperty("ProductId")
                .GetGuid());
    }

    [Fact]
    public async Task SaveChangesAsync_WhenProductSaveFails_DoesNotPersistProductOrOutboxAndKeepsDomainEvent()
    {
        // Arrange
        await using var scope = fixture.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<CatalogDbContext>();

        var missingCategoryId = Guid.NewGuid();

        var product = Product.Create(
            ProductName.Create($"Invalid Product {Guid.NewGuid():N}"),
            "Product with a missing category",
            missingCategoryId,
            Price.Create(100));

        product.Activate();
        dbContext.Products.Add(product);

        // Act
        await Assert.ThrowsAsync<DbUpdateException>(
            () => dbContext.SaveChangesAsync(CancellationToken.None));

   
        Assert.Single(product.DomainEvents);

        // Assert using a fresh DbContext, so we inspect persisted data.
        await using var verificationScope =
            fixture.Services.CreateAsyncScope();

        var verificationDbContext = verificationScope.ServiceProvider
            .GetRequiredService<CatalogDbContext>();

        var productExists = await verificationDbContext.Products
            .AsNoTracking()
            .AnyAsync(x => x.Id == product.Id,CancellationToken.None);

        Assert.False(productExists);

        var outboxMessages = await verificationDbContext
            .Set<OutboxMessage>()
            .AsNoTracking()
            .Where(x => x.Type == "ProductActivated")
            .ToListAsync(CancellationToken.None);

        var matchingMessageExists = outboxMessages.Any(message =>
        {
            using var payload = JsonDocument.Parse(message.Payload);

            return payload.RootElement
                .GetProperty("ProductId")
                .GetGuid() == product.Id;
        });

        Assert.False(matchingMessageExists);
    }
}
