using Catalog.Application.Abstractions.Persistence.Repositories.Queries;
using MediatR;

namespace Catalog.Application.Category.Queries.BackOffice.GetCategoryById;

public sealed class GetCategoryByIdQueryHandler
    : IRequestHandler<GetCategoryByIdQuery, CategoryDetailsResult?>
{
    private readonly ICategoryReadService _categoryReadService;

    public GetCategoryByIdQueryHandler(
        ICategoryReadService categoryReadService)
    {
        _categoryReadService = categoryReadService;
    }

    public async Task<CategoryDetailsResult?> Handle(
        GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        return await _categoryReadService.GetByIdAsync(
            request.CategoryId, cancellationToken);

    }
}
