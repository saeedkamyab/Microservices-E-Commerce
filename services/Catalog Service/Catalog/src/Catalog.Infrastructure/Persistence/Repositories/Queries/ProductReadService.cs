using Catalog.Application.Abstractions.Persistence.Repositories.Queries;
using Catalog.Application.Common;
using Catalog.Application.Common.Enums;
using Catalog.Application.Product.Queries.BackOffice.GetProductById;
using Catalog.Application.Product.Queries.BackOffice.GetProducts;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using static Catalog.Application.Common.Enums.SortEnum;

namespace Catalog.Infrastructure.Persistence.Repositories.Queries;

internal sealed class ProductReadService : IProductReadService
{
    private readonly CatalogDbContext _dbContext;

    public ProductReadService(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<ProductListItem>> GetProductsAsync(
        string? search,
        ProductStatusFilter? status,
        ProductSortBy sortBy,
        SortDirection sortDirection,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Products
            .AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(
                    x.Name.Value,
                    $"%{search}%"));
        }
        if (status.HasValue)
        {
            query = ApplyStatusFilter(query, status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = (sortBy, sortDirection) switch
        {
            (ProductSortBy.Name, SortDirection.Asc) =>
                query
                    .OrderBy(x => x.Name.Value)
                    .ThenBy(x => x.Id),

            (ProductSortBy.Name, SortDirection.Desc) =>
                query
                    .OrderByDescending(x => x.Name.Value)
                    .ThenBy(x => x.Id),

            (ProductSortBy.Status, SortDirection.Asc) =>
                query
                    .OrderBy(x => x.Status)
                    .ThenBy(x => x.Name.Value)
                    .ThenBy(x => x.Id),

            (ProductSortBy.Status, SortDirection.Desc) =>
                query
                    .OrderByDescending(x => x.Status)
                    .ThenBy(x => x.Name.Value)
                    .ThenBy(x => x.Id),


            (ProductSortBy.Price, SortDirection.Asc) =>
     query
         .OrderBy(x => x.Price)
         .ThenBy(x => x.Status)
         .ThenBy(x => x.Name.Value)
         .ThenBy(x => x.Id),

            (ProductSortBy.Price, SortDirection.Desc) =>
                query
                    .OrderByDescending(x => x.Price)
                    .ThenBy(x => x.Status)
                    .ThenBy(x => x.Name.Value)
                    .ThenBy(x => x.Id),

            _ =>
                query
                    .OrderBy(x => x.Name.Value)
                    .ThenBy(x => x.Id)
        };


        var items = await query
      .Skip((pageNumber - 1) * pageSize)
      .Take(pageSize)
      .Select(x => new ProductListItem(
          x.Id,
          x.Name.Value,
          x.ProductDescription,
          x.CategoryId,
          x.Status.ToString(),
          x.Price.Amount,

     _dbContext.Categories
        .Where(c => c.Id == x.CategoryId)
        .Select(c => c.CategoryName.Value)
        .First()
        
        )).ToListAsync(cancellationToken);

        return new PagedResult<ProductListItem>(
            items,
            totalCount,
            pageNumber,
            pageSize);
    }



    public async Task<ProductDetailsResult?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Products
        .AsNoTracking()
        .Where(x => x.Id == id)
        .Select(product => new ProductDetailsResult(
            product.Id,
            product.Name.Value,
            product.ProductDescription,
            product.CategoryId,
            product.Price.Amount,
            product.Status.ToString(),
            product.Specifications
                .Select(x => new ProductSpecificationResult(
                    x.AttributeDefinitionId,
                    FormatSpecificationValue(x.Value)))
                .ToArray()))
        .FirstOrDefaultAsync(cancellationToken);
    }




    private static IQueryable<Product> ApplyStatusFilter(
           IQueryable<Product> query,
           ProductStatusFilter status)
    {
        return status switch
        {
            ProductStatusFilter.Active =>
                query.Where(x =>
                    x.Status == ProductStatus.Active),

            ProductStatusFilter.Inactive =>
                query.Where(x =>
                    x.Status == ProductStatus.Inactive),

            _ => throw new ArgumentOutOfRangeException(
                nameof(status),
                status,
                "Unsupported category status filter.")
        };
    }


    private static string FormatSpecificationValue(
        ProductSpecificationValue value)
    {
        return value.Type switch
        {
            AttributeType.Text =>
                (string)value.Value,

            AttributeType.Number =>
                ((int)value.Value).ToString(),

            AttributeType.Decimal =>
                ((decimal)value.Value).ToString(),

            AttributeType.Boolean =>
                ((bool)value.Value).ToString(),

            AttributeType.Date =>
                ((DateTime)value.Value).ToString(),

            AttributeType.Option =>
                ((AttributeOption)value.Value).Value,

            _ => throw new ArgumentOutOfRangeException()
        };
    }

}