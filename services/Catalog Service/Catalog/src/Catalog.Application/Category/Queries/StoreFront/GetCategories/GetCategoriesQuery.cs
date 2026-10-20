using Catalog.Application.Common;
using MediatR;

namespace Catalog.Application.Category.Queries.StoreFront.GetCategories;

public sealed record GetCategoriesQuery(
    string? Search,
    string? SortBy,
    string? SortDirection,
    int PageNumber,
    int PageSize)
    : IRequest<PagedResult<CategoryListItem>>;