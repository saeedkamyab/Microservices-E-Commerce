using Catalog.Application.Abstractions.Persistence.Repositories.Queries;
using Catalog.Application.Category.Queries.GetCategories;
using Catalog.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories.Queries;

internal sealed class CategoryReadService : ICategoryReadService
{
    private readonly CatalogDbContext _dbContext;

    public CategoryReadService(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<CategoryListItem>> GetPagedAsync(
        string? search,
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

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.CategoryName.Value)
            .ThenBy(x => x.Id)
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
}