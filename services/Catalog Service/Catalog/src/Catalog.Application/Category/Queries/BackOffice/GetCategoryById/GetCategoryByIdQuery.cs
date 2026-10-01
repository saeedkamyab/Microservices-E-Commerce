using MediatR;

namespace Catalog.Application.Category.Queries.BackOffice.GetCategoryById;

public sealed record GetCategoryByIdQuery(
    Guid CategoryId
) : IRequest<CategoryDetailsResult?>;