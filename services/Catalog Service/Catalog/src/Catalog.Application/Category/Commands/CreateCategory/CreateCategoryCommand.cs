using MediatR;

namespace Catalog.Application.Category.Commands.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    Guid? ParentCategoryId
    ) : IRequest<Guid>
{
   
}