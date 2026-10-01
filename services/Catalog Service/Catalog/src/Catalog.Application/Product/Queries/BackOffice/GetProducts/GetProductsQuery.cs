using Catalog.Application.Common;
using MediatR;

namespace Catalog.Application.Product.Queries.BackOffice.GetProducts;

public sealed record GetProductsQuery(
    string? Search,
    string? Status,
    string? SortBy,
    string? SortDirection,
    int PageNumber,
    int PageSize)
    : IRequest<PagedResult<ProductListItem>>;