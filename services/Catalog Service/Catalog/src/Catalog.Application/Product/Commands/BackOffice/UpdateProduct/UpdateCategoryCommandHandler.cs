using Catalog.Application.Abstractions.Persistence;
using Catalog.Application.Abstractions.Persistence.Repositories;
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
        IProductRepository productRepository,ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException(
                $"Product with id {request.ProductId} not found.");
        }

        product.Rename(ProductName.Create(request.Name));
        product.ChangeDescription(request.ProductDescription);
        product.ChangePrice(Price.Create(request.Price));

        if (product.CategoryId != request.CategoryId)
        {
                var categoryId = request.CategoryId;

                var category = await _categoryRepository.GetByIdAsync(
                    categoryId, cancellationToken);

                if (category is null)
                {
                    throw new NotFoundException("Category was not found.");
                }
                if (category.Status != CategoryStatus.Active)
                {
                    throw new InvalidOperationException(
                        "Cannot create a product for an inactive category.");
                }
        }
        product.ChangeCategory(request.CategoryId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
