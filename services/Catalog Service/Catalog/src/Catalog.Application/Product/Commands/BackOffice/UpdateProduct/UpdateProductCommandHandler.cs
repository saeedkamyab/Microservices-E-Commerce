using Catalog.Application.Abstractions.Persistence;
using Catalog.Application.Abstractions.Persistence.Repositories;
using Catalog.Application.Common;
using Catalog.Application.Exceptions;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.ValueObjects;
using MediatR;

namespace Catalog.Application.Product.Coammands.BackOffice.UpdateProduct;

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(
        IProductRepository productRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdWithSpecificationsAsync(
            request.ProductId, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException(
                $"Product with id {request.ProductId} not found.");
        }

        var category = await _categoryRepository.GetWithAttributeDefinitionsAsync(
            request.CategoryId, cancellationToken);


        if (category is null)
        {
            throw new NotFoundException("Category was not found.");
        }

        if (category.Status != CategoryStatus.Active)
        {
            throw new InvalidOperationException(
                   "Cannot assign a product to an inactive category.");
        }


        product.Rename(ProductName.Create(request.Name));
        product.ChangeDescription(request.ProductDescription);
        product.ChangePrice(Price.Create(request.Price));
        product.ChangeCategory(request.CategoryId);



       ApplySpecificationChanges(request, product, category);

        await _productRepository.SyncSpecificationsAsync(
    product,
    cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }


    private static void ApplySpecificationChanges(UpdateProductCommand request, Domain.Entities.Product product,
        Domain.Entities.Category category)
    {

        var requestedAttributeIds = request.Specifications
      .Select(x => x.AttributeDefinitionId)
      .ToHashSet();

        var existingAttributeIds = product.Specifications
     .Select(x => x.AttributeDefinitionId)
     .ToHashSet();

        var missingRequiredAttribute =
     category.AttributeDefinitions
         .FirstOrDefault(x =>
             x.IsRequired &&
             !requestedAttributeIds.Contains(x.Id));

        if (missingRequiredAttribute is not null)
        {
            throw new ConflictException(
                $"Required attribute '{missingRequiredAttribute.Name}' was not provided.");
        }


        var removedSpecifications = product.Specifications
      .Where(x =>
          !requestedAttributeIds.Contains(
              x.AttributeDefinitionId))
      .ToList();


        //Remove specifications that are not in the request
        foreach (var specification in removedSpecifications)
        {
            product.RemoveSpecification(
                specification.AttributeDefinitionId);
        }

        // ADD / CHANGE
        foreach (var input in request.Specifications)
        {
            var definition = category.AttributeDefinitions
                .FirstOrDefault(x =>
                    x.Id == input.AttributeDefinitionId);

            if (definition is null)
            {
                throw new InvalidOperationException(
                    $"Attribute definition '{input.AttributeDefinitionId}' " +
                    "does not belong to the selected category.");
            }
            ProductSpecificationValue value;

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
                value = ProductSpecificationValue.CreateOption(option);
            }
            else
            {
                value = ProductSpecificationValueFactory.Create(
           definition.Type,
           input.Value);

            }
            definition.ValidateValue(value);

            if (existingAttributeIds.Contains(
         input.AttributeDefinitionId))
            {
                product.ChangeSpecification(
                    input.AttributeDefinitionId,
                    value);
            }
            else
            {
                var specification =
                    ProductSpecification.Create(
                        definition.Id,
                        value);

                product.AddSpecification(specification);
            }
        }
    }
}
