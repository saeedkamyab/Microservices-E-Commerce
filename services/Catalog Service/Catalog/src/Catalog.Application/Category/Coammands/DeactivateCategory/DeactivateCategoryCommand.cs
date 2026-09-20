using MediatR;

namespace Catalog.Application.Category.Coammands.DeactivateCategory;

public sealed record DeactivateCategoryCommand(Guid CategoryId) : IRequest;

