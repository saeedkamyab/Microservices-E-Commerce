using Catalog.Application.Abstractions.Persistence;
using Catalog.Application.Abstractions.Persistence.Repositories;
using Catalog.Application.Common;
using Catalog.Application.Exceptions;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.ValueObjects;
using MediatR;

namespace Catalog.Application.Product.Coammands.BackOffice.CreateProduct;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {

        var category = await _categoryRepository.GetWithAttributeDefinitionsAsync(
            request.CategoryId, cancellationToken);

        if (category is null)
        {
            throw new NotFoundException("Category was not found.");
        }
        if (category.Status != CategoryStatus.Active)
        {
            throw new InvalidOperationException(
                "Cannot create a product for an inactive category.");
        }

        var requestedAttributeIds = request.Specifications.Select(x => x.AttributeDefinitionId).ToHashSet();

        var missingRequiredAttribute = category.AttributeDefinitions
   .FirstOrDefault(x =>
       x.IsRequired &&
       !requestedAttributeIds.Contains(x.Id));

        if (missingRequiredAttribute is not null)
        {
            throw new ConflictException(
                $"Required attribute '{missingRequiredAttribute.Name}' was not provided.");
        }

        var product = Domain.Entities.Product.Create(
            ProductName.Create(request.Name),
            request.Description,
            request.CategoryId,
            Price.Create(request.Price));

        foreach (var input in request.Specifications)
        {
            var definition =
                category.AttributeDefinitions
                    .FirstOrDefault(x =>
                        x.Id == input.AttributeDefinitionId);

            if (definition is null)
            {
                throw new InvalidOperationException(
                    "Attribute definition does not belong to category.");
            }
            if (definition.Type == AttributeType.Option)
            {
                var option = definition.Options
                    .FirstOrDefault(x =>
                        x.Value.Equals(
                            input.Value,
                            StringComparison.OrdinalIgnoreCase));

                if (option is null)
                {
                    throw new InvalidOperationException(
                        $"'{input.Value}' is not a valid option for attribute '{definition.Name}'.");
                }

                product.AddSpecification(
            ProductSpecification.Create(
                definition.Id,
                ProductSpecificationValue.CreateOption(option)));


                continue;
            }

            var value =
                ProductSpecificationValueFactory.Create(
                    definition.Type,
                    input.Value);

            var specification =
                ProductSpecification.Create(
                    definition.Id,
                    value);

            product.AddSpecification(specification);
        }


        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
}
