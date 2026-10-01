using Catalog.Application.Abstractions.Persistence;
using Catalog.Application.Abstractions.Persistence.Repositories;
using Catalog.Application.Exceptions;
using MediatR;

namespace Catalog.Application.Product.Coammands.BackOffice.ActivateProduct;

public sealed class ActivateProductCommandHandler : IRequestHandler<ActivateProductCommand>
{
    private readonly IProductRepository _producttRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateProductCommandHandler(
        IProductRepository producttRepository,
        IUnitOfWork unitOfWork)
    {
        _producttRepository = producttRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ActivateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _producttRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException(
                $"Product with ID {request.ProductId} not found.");
        }
        product.Activate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
