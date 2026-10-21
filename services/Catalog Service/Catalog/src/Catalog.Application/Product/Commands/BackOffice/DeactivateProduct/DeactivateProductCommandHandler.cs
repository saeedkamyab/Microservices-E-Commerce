using Catalog.Application.Abstractions.Caching;
using Catalog.Application.Abstractions.Persistence;
using Catalog.Application.Abstractions.Persistence.Repositories;
using Catalog.Application.Common.Caching;
using Catalog.Application.Exceptions;
using MediatR;

namespace Catalog.Application.Product.Coammands.BackOffice.DeactivateProduct;

public sealed class DeactivateProductCommandHandler : IRequestHandler<DeactivateProductCommand>
{
    private readonly IProductRepository _producttRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheVersionService _cacheVersionService;
    public DeactivateProductCommandHandler(
        IProductRepository producttRepository,
        IUnitOfWork unitOfWork,
        ICacheVersionService cacheVersionService)
    {
        _producttRepository = producttRepository;
        _unitOfWork = unitOfWork;
        _cacheVersionService = cacheVersionService;
    }

    public async Task Handle(DeactivateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _producttRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException(
                $"Product with ID {request.ProductId} not found.");
        }
        product.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheVersionService.IncrementVersionAsync(
         CatalogCacheKeys.backOfficeProductsVersion,
         cancellationToken);

    }
}
