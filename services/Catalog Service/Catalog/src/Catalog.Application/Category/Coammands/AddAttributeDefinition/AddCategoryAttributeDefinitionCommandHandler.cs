using Catalog.Application.Abstractions.Persistence;
using Catalog.Application.Abstractions.Persistence.Repositories;
using Catalog.Application.Common;
using Catalog.Application.Exceptions;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.ValueObjects;
using MediatR;

namespace Catalog.Application.Category.Coammands.AddAttributeDefinition;

public sealed class AddCategoryAttributeDefinitionCommandHandler
    : IRequestHandler<AddCategoryAttributeDefinitionCommand, Guid>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddCategoryAttributeDefinitionCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        AddCategoryAttributeDefinitionCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(
            request.CategoryId,
            cancellationToken);

        if (category is null)
            throw new NotFoundException(
                $"Category with id {request.CategoryId} was not found.");

        var exists =
    await _categoryRepository.AttributeDefinitionNameExistsAsync(
        category.Id,
        request.Name,
        cancellationToken);


        if (exists)
            throw new ConflictException(
                "An attribute with the same name already exists.");

        var definition = CategoryAttributeDefinition.Create(
            Name.Create(request.Name),
            StringToEnumConvertor.ToEnum<AttributeType>(request.Type),
            request.IsRequired);

        foreach (var option in request.Options)
        {
            definition.AddOption(
                AttributeOption.Create(option));
        }

        category.AddAttributeDefinition(definition);

        await _categoryRepository.AddCategoryAttributeDefinitionAsync(category, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return definition.Id;
    }
}
