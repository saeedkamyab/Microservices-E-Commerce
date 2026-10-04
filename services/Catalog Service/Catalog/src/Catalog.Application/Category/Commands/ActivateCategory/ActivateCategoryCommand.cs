using MediatR;

namespace Catalog.Application.Category.Commands.ActivateCategory;

public sealed record ActivateCategoryCommand(Guid CategoryId) : IRequest;

