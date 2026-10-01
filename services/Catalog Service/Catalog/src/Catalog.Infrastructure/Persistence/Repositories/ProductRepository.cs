using Catalog.Application.Abstractions.Persistence.Repositories;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

internal sealed class ProductRepository : IProductRepository
{
    private readonly CatalogDbContext _dbContext;

    public ProductRepository(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(
            x => x.Id == id, cancellationToken);
        if (product is null)
            return null;
        return product;
    }

    //public async Task<Product?> GetWithAttributeDefinitionsAsync(Guid id, CancellationToken cancellationToken)
    //{
    //    var product = await _dbContext.Categories
    //        .FirstOrDefaultAsync(
    //            x => x.Id == id,
    //            cancellationToken);

    //    if (product is null)
    //        return null;

    //    var definitions = await _dbContext.ProductAttributeDefinitions
    //        .AsNoTracking()
    //        .Where(x => x.ProductId == id)
    //        .ToListAsync(cancellationToken);

    //    if (definitions.Count == 0)
    //        return product;

    //    var definitionIds = definitions
    //        .Select(x => x.Id)
    //        .ToArray();

    //    var options = await _dbContext.AttributeOptions
    //        .AsNoTracking()
    //        .Where(x => definitionIds.Contains(x.AttributeDefinitionId))
    //        .ToListAsync(cancellationToken);

    //    var optionsByDefinitionId = options
    //        .GroupBy(x => x.AttributeDefinitionId)
    //        .ToDictionary(x => x.Key, x => x.ToList());

    //    foreach (var definitionRecord in definitions)
    //    {
    //        var definition = ProductAttributeDefinition.Rehydrate(
    //            definitionRecord.Id,
    //            Name.Create(definitionRecord.Name),
    //            definitionRecord.Type,
    //            definitionRecord.IsRequired);

    //        if (optionsByDefinitionId.TryGetValue(
    //                definitionRecord.Id,
    //                out var definitionOptions))
    //        {
    //            foreach (var optionRecord in definitionOptions)
    //            {
    //                definition.AddOption(
    //                    AttributeOption.Create(optionRecord.Value));
    //            }
    //        }

    //        product.AddAttributeDefinition(definition);
    //    }

    //    return product;
    //}

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        await _dbContext.Products.AddAsync(
           product,
           cancellationToken);

        //foreach (var definition in product.AttributeDefinitions)
        //{
        //    await _dbContext.ProductAttributeDefinitions.AddAsync(
        //                   new ProductAttributeDefinitionRecord
        //                   {
        //                       Id = definition.Id,
        //                       ProductId = product.Id,
        //                       Name = definition.Name.Value,
        //                       Type = definition.Type,
        //                       IsRequired = definition.IsRequired
        //                   },
        //                   cancellationToken);

        //    foreach (var option in definition.Options)
        //    {
        //        await _dbContext.AttributeOptions.AddAsync(
        //            new AttributeOptionRecord
        //            {
        //                Id = Guid.NewGuid(),
        //                AttributeDefinitionId = definition.Id,
        //                Value = option.Value
        //            },
        //            cancellationToken);
        //    }
        //}
    }


}
