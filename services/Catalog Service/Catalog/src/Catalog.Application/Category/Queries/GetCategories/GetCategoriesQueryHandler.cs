using Catalog.Application.Abstractions.Persistence.Repositories.Queries;
using Catalog.Application.Common;
using MediatR;

namespace Catalog.Application.Category.Queries.GetCategories;

public sealed class GetCategoriesQueryHandler
       : IRequestHandler<
        GetCategoriesQuery,
        PagedResult<CategoryListItem>>
{
    private readonly ICategoryReadService _categoryReadService;

    public GetCategoriesQueryHandler(
        ICategoryReadService categoryReadService)
    {
        _categoryReadService = categoryReadService;
    }

    public Task<PagedResult<CategoryListItem>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        return _categoryReadService.GetPagedAsync(
            request.Search,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
