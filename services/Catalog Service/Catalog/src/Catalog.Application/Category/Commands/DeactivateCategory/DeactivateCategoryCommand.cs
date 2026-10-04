using MediatR;

namespace Catalog.Application.Category.Commands.DeactivateCategory;

public sealed record DeactivateCategoryCommand(Guid CategoryId) : IRequest;

