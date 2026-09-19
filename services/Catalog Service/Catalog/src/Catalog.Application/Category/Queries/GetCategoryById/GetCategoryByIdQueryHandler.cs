using Catalog.Application.Abstractions.Persistence.Repositories;
using MediatR;

namespace Catalog.Application.Category.Queries.GetCategoryById;

public sealed class GetCategoryByIdQueryHandler
    : IRequestHandler<GetCategoryByIdQuery, CategoryDetailsResult?>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdQueryHandler(
        ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDetailsResult?> Handle(
        GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(
              request.CategoryId,
              cancellationToken);
        if (category is null)
            return null;


        return new CategoryDetailsResult(
            category.Id,
            category.CategoryName.Value,
            category.ParentCategoryId,
            category.Status.ToString(),
            category.AttributeDefinitions
                .Select(x =>
                    new CategoryAttributeResult(
                        x.Id,
                        x.Name.Value,
                        x.Type,
                        x.IsRequired,
                        x.Options
                            .Select(o => o.Value)
                            .ToArray()))
                .ToArray());

    }
}
