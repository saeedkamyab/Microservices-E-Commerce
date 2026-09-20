using MediatR;

namespace Catalog.Application.Category.Coammands.ActivateCategory;

public sealed record ActivateCategoryCommand(Guid CategoryId) : IRequest;

