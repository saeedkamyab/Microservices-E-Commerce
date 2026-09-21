using MediatR;

namespace Catalog.Application.Category.Coammands.UpdateCategory;

public sealed record UpdateCategoryCommand(
    Guid CategoryId,
    string Name,
    Guid? ParentCategoryId
    ) : IRequest;

