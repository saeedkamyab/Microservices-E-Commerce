using Catalog.Application.Abstractions.Persistence.Repositories.Queries;
using Catalog.Application.Category.Queries.BackOffice.GetCategories;
using Catalog.Application.Category.Queries.BackOffice.GetCategoryById;
using Catalog.Application.Common;
using Catalog.Application.Common.Enums;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using static Catalog.Application.Common.Enums.SortEnum;

namespace Catalog.Infrastructure.Persistence.Repositories.Queries;

internal sealed class CategoryReadService : ICategoryReadService
{
    private readonly CatalogDbContext _dbContext;

    public CategoryReadService(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<CategoryListItem>> GetCategoriesAsync(
        string? search,
        CategoryStatusFilter? status,
        CategorySortBy sortBy,
        SortDirection sortDirection,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Categories
            .AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(
                    x.CategoryName.Value,
                    $"%{search}%"));
        }
        if (status.HasValue)
        {
            query = ApplyStatusFilter(query, status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = (sortBy, sortDirection) switch
        {
            (CategorySortBy.Name, SortDirection.Asc) =>
                query
                    .OrderBy(x => x.CategoryName.Value)
                    .ThenBy(x => x.Id),

            (CategorySortBy.Name, SortDirection.Desc) =>
                query
                    .OrderByDescending(x => x.CategoryName.Value)
                    .ThenBy(x => x.Id),

            (CategorySortBy.Status, SortDirection.Asc) =>
                query
                    .OrderBy(x => x.Status)
                    .ThenBy(x => x.CategoryName.Value)
                    .ThenBy(x => x.Id),

            (CategorySortBy.Status, SortDirection.Desc) =>
                query
                    .OrderByDescending(x => x.Status)
                    .ThenBy(x => x.CategoryName.Value)
                    .ThenBy(x => x.Id),

            _ =>
                query
                    .OrderBy(x => x.CategoryName.Value)
                    .ThenBy(x => x.Id)
        };


        var items = await query
      .Skip((pageNumber - 1) * pageSize)
      .Take(pageSize)
      .Select(x => new CategoryListItem(
          x.Id,
          x.CategoryName.Value,
          x.Status.ToString(),
          x.ParentCategoryId,

          x.ParentCategoryId == null
              ? null
              : _dbContext.Categories
                  .Where(parent =>
                      parent.Id == x.ParentCategoryId)
                  .Select(parent =>
                      parent.CategoryName.Value)
                  .FirstOrDefault()))
      .ToListAsync(cancellationToken);

        return new PagedResult<CategoryListItem>(
            items,
            totalCount,
            pageNumber,
            pageSize);
    }



    public async Task<CategoryDetailsResult?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {

        var category = await _dbContext.Categories.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new
            {
                Id = x.Id,
                Name = x.CategoryName.Value,
                Status = x.Status,
                ParentCategoryId = x.ParentCategoryId
            })
            .FirstOrDefaultAsync();

        if (category is null)
            return null;

        var definitions = await _dbContext.CategoryAttributeDefinitions
       .AsNoTracking()
       .Where(x => x.CategoryId == id)
       .ToListAsync(cancellationToken);

        var definitionIds = definitions
    .Select(x => x.Id)
    .ToArray();

        var options = await _dbContext.AttributeOptions
       .AsNoTracking()
       .Where(x => definitionIds.Contains(x.AttributeDefinitionId))
       .ToListAsync(cancellationToken);

        var attributes = definitions
        .Select(definition => new CategoryAttributeResult(
            definition.Id,
            definition.Name,
            definition.Type.ToString(),
            definition.IsRequired,
            options
                .Where(x =>
                    x.AttributeDefinitionId == definition.Id)
                .Select(x => x.Value)
                .ToArray()))
        .ToArray();

        return new CategoryDetailsResult(
             category.Id,
             category.Name,
             category.ParentCategoryId,
             category.Status.ToString(),
             attributes);
    }




    private static IQueryable<Category> ApplyStatusFilter(
           IQueryable<Category> query,
           CategoryStatusFilter status)
    {
        return status switch
        {
            CategoryStatusFilter.Active =>
                query.Where(x =>
                    x.Status == CategoryStatus.Active),

            CategoryStatusFilter.Inactive =>
                query.Where(x =>
                    x.Status == CategoryStatus.Inactive),

            _ => throw new ArgumentOutOfRangeException(
                nameof(status),
                status,
                "Unsupported category status filter.")
        };
    }
}