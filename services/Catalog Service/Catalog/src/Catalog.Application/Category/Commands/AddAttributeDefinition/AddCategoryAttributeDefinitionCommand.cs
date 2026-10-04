using MediatR;

namespace Catalog.Application.Category.Commands.AddAttributeDefinition;

public sealed record AddCategoryAttributeDefinitionCommand(
    Guid CategoryId,
    string Name,
    string Type,
    bool IsRequired,
    IReadOnlyCollection<string> Options) : IRequest<Guid>;
