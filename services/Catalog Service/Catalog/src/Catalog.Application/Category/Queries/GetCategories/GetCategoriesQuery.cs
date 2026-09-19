using Catalog.Application.Common;
using MediatR;

namespace Catalog.Application.Category.Queries.GetCategories;

public sealed record GetCategoriesQuery(
    string? Search,
    int PageNumber,
    int PageSize)
    : IRequest<PagedResult<CategoryListItem>>;