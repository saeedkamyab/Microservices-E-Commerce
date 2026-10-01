using Catalog.Application.Abstractions.Persistence;
using Catalog.Application.Abstractions.Persistence.Repositories;
using Catalog.Application.Exceptions;
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

        var category = await _categoryRepository.GetByIdAsync(
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

        var product = Domain.Entities.Product.Create(
            ProductName.Create(request.Name),
            request.ProductDescription,
            request.CategoryId,
            Price.Create(request.Price));

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
}
