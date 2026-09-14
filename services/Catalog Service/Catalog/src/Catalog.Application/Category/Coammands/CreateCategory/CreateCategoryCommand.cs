using Catalog.Domain.Enums;
using MediatR;
using System.Net;

namespace Catalog.Application.Category.Coammands.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    Guid? ParentCategoryId,
    IReadOnlyCollection<CategoryAttributeDefinitionInput> Attributes
    ) : IRequest<Guid>;


public sealed record CategoryAttributeDefinitionInput(
    string Name,
    AttributeType Type,
    bool IsRequired,
    IReadOnlyCollection<string>Options);
