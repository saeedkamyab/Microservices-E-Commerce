using Catalog.Application.Abstractions.Persistence;
using Catalog.Application.Abstractions.Persistence.Repositories;
using Catalog.Application.Exceptions;
using MediatR;

namespace Catalog.Application.Category.Commands.RemoveAttributeDefinition;

public sealed class RemoveAttributeDefinitionCommandHandler : IRequestHandler<RemoveAttributeDefinitionCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveAttributeDefinitionCommandHandler(
        ICategoryRepository categoryRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveAttributeDefinitionCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetWithAttributeDefinitionsAsync(
            request.CategoryId,
            cancellationToken);

        if (category is null)
        {
            throw new NotFoundException(
                $"Category with ID {request.CategoryId} not found.");
        }

        if (!category.AttributeDefinitions.Any(ad => ad.Id == request.AttributeId))
        {
            throw new NotFoundException(
                $"Attribute definition with ID {request.AttributeId} not found in category with ID {request.CategoryId}.");
        }

        var isUsed =
       await _productRepository.AnyProductUsesAttributeDefinitionAsync(
           request.AttributeId,
           cancellationToken);

        if (isUsed)
            throw new ConflictException(
                "The attribute definition cannot be removed because it is used by one or more products.");


        category.RemoveAttributeDefinition(request.AttributeId);

        await _categoryRepository.SyncRemovedAttributeDefinitionsAsync(
    category,
    cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
