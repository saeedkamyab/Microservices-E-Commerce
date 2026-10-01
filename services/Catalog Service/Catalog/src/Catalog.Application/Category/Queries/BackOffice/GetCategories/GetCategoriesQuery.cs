using Catalog.Application.Common;
using MediatR;

namespace Catalog.Application.Category.Queries.BackOffice.GetCategories;

public sealed record GetCategoriesQuery(
    string? Search,
    string? Status,
    string? SortBy,
    string? SortDirection,
    int PageNumber,
    int PageSize)
    : IRequest<PagedResult<CategoryListItem>>;