using MediatR;

namespace Catalog.Application.Category.Commands.RemoveAttributeDefinition;

public sealed record RemoveAttributeDefinitionCommand(Guid CategoryId, Guid AttributeId) : IRequest;

