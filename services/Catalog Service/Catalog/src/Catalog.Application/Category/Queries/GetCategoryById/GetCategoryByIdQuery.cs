using MediatR;

namespace Catalog.Application.Category.Queries.GetCategoryById;

public sealed record GetCategoryByIdQuery(
    Guid CategoryId
) : IRequest<CategoryDetailsResult?>;