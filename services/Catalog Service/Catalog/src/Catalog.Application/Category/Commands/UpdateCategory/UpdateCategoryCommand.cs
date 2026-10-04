using MediatR;

namespace Catalog.Application.Category.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(
    Guid CategoryId,
    string Name,
    Guid? ParentCategoryId
    ) : IRequest;

