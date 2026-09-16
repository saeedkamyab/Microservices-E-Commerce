using Catalog.Application.Abstractions.Persistence;
using Catalog.Application.Abstractions.Persistence.Repositories;
using Catalog.Application.Exceptions;
using Catalog.Domain.Entities;
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
                throw new NotFoundException("Parent category was not found.");
            }
        }
        var category = Domain.Entities.Category.Create(
            CategoryName.Create(request.Name),
            request.ParentCategoryId);

            foreach (var attribute in request.Attributes)
            {
                var attributeDefinition = CategoryAttributeDefinition.Create(
                    Name.Create(attribute.Name),
                    attribute.Type,
                    attribute.IsRequired);

                foreach (var option in attribute.Options)
                {
                    attributeDefinition.AddOption(AttributeOption.Create(option));
                }

                category.AddAttributeDefinition(attributeDefinition);
            }



        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return category.Id;
    }
}
