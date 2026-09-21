using Catalog.Application.Abstractions.Persistence.Repositories;
using Catalog.Domain.Entities;
using Catalog.Domain.ValueObjects;
using Catalog.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Catalog.Infrastructure.Persistence.Repositories;

internal sealed class CategoryRepository : ICategoryRepository
{
    private readonly CatalogDbContext _dbContext;

    public CategoryRepository(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(
            x => x.Id == id, cancellationToken);
        if (category is null)
            return null;
        return category;
    }

    public Task<Category?> GetWithAttributeDefinitionsAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    //public async Task<Category?> GetByIdAsync(
    // Guid id,
    // CancellationToken cancellationToken)
    //{
    //    var category = await _dbContext.Categories
    //        .AsNoTracking()
    //        .FirstOrDefaultAsync(
    //            x => x.Id == id,
    //            cancellationToken);

    //    if (category is null)
    //        return null;

    //    var definitions = await _dbContext.CategoryAttributeDefinitions
    //        .AsNoTracking()
    //        .Where(x => x.CategoryId == id)
    //        .ToListAsync(cancellationToken);

    //    if (definitions.Count == 0)
    //        return category;

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
    //        var definition = CategoryAttributeDefinition.Rehydrate(
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

    //        category.AddAttributeDefinition(definition);
    //    }

    //    return category;
    //}


    public async Task<bool> WouldCreateCycleAsync(
    Guid categoryId,
    Guid newParentId,
    CancellationToken cancellationToken)
    {
        const string sql = """
        WITH RECURSIVE ancestors AS
        (
            SELECT id, parent_category_id
            FROM categories
            WHERE id = @newParentId

            UNION ALL

            SELECT c.id, c.parent_category_id
            FROM categories c
            INNER JOIN ancestors a
                ON c.id = a.parent_category_id
        )
        SELECT EXISTS
        (
            SELECT 1
            FROM ancestors
            WHERE id = @categoryId
        );
        """;

        var connection = _dbContext.Database.GetDbConnection();

        var shouldCloseConnection = connection.State != ConnectionState.Open;

        if (shouldCloseConnection)
            await connection.OpenAsync(cancellationToken);

        try
        {

            await using var command = connection.CreateCommand();

            command.CommandText = sql;

            var newParentParameter = command.CreateParameter();
            newParentParameter.ParameterName = "@newParentId";
            newParentParameter.Value = newParentId;
            command.Parameters.Add(newParentParameter);

            var categoryParameter = command.CreateParameter();
            categoryParameter.ParameterName = "@categoryId";
            categoryParameter.Value = categoryId;
            command.Parameters.Add(categoryParameter);

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);

            var result = await command.ExecuteScalarAsync(cancellationToken);

            return result is true;
        }
        finally
        {
            if (shouldCloseConnection)
                await connection.CloseAsync();
        }
    }

    public async Task AddAsync(Category category, CancellationToken cancellationToken)
    {
        await _dbContext.Categories.AddAsync(
           category,
           cancellationToken);

        foreach (var definition in category.AttributeDefinitions)
        {
            await _dbContext.CategoryAttributeDefinitions.AddAsync(
                           new CategoryAttributeDefinitionRecord
                           {
                               Id = definition.Id,
                               CategoryId = category.Id,
                               Name = definition.Name.Value,
                               Type = definition.Type,
                               IsRequired = definition.IsRequired
                           },
                           cancellationToken);

            foreach (var option in definition.Options)
            {
                await _dbContext.AttributeOptions.AddAsync(
                    new AttributeOptionRecord
                    {
                        Id = Guid.NewGuid(),
                        AttributeDefinitionId = definition.Id,
                        Value = option.Value
                    },
                    cancellationToken);
            }
        }
    }


}
