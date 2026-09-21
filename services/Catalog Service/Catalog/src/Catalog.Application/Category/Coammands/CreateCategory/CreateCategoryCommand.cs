using Catalog.Domain.Enums;
using MediatR;
using System.Net;

namespace Catalog.Application.Category.Coammands.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    Guid? ParentCategoryId
    ) : IRequest<Guid>;



