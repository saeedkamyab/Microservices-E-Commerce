using Catalog.Application.Abstractions.Persistence;
using Catalog.Application.Abstractions.Persistence.Repositories;
using Catalog.Application.Exceptions;
using MediatR;

namespace Catalog.Application.Product.Coammands.BackOffice.DeactivateProduct;

public sealed class DeactivateProductCommandHandler : IRequestHandler<DeactivateProductCommand>
{
    private readonly IProductRepository _producttRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateProductCommandHandler(
        IProductRepository producttRepository,
        IUnitOfWork unitOfWork)
    {
        _producttRepository = producttRepository;
        _unitOfWork = unitOfWork;
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
    }
}
