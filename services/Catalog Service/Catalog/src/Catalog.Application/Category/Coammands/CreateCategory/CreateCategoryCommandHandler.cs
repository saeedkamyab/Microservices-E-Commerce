using Catalog.Application.Abstractions.Persistence;
using Catalog.Application.Abstractions.Persistence.Repositories;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.ValueObjects;
using MediatR;

namespace Catalog.Application.Category.Coammands.CreateCategory;

public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.ParentCategoryId.HasValue)
        {
            var parentCategory = await _categoryRepository.GetByIdAsync(
                request.ParentCategoryId.Value, cancellationToken);
            if (parentCategory is null)
            {
                throw new InvalidOperationException("Parent category was not found.");
            }
        }
        var category = Domain.Entities.Category.Create(
            CategoryName.Create(request.Name),
            request.ParentCategoryId);

        foreach (var attribute in request.Attributes)
        {
            var attributeDefiniation = CategoryAttributeDefinition.Create(
                Name.Create(attribute.Name),
                attribute.Type,
                attribute.IsRequired);
            if (attribute.Type == AttributeType.Option)
            {
                if (attribute.Options.Count == 0)
                {
                    throw new InvalidOperationException(
                        $"Option attribute '{attribute.Name}' must have at least one option.");
                }
                foreach (var option in attribute.Options)
                {
                    attributeDefiniation.AddOption(AttributeOption.Create(option));
                }
            }
            else if (attribute.Options.Count > 0)
            {
                throw new InvalidOperationException(
                     $"Attribute '{attribute.Name}' cannot have options because it is not of type Option.");
            }
            category.AddAttributeDefinition(attributeDefiniation);
        }
        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return category.Id;
    }
}
