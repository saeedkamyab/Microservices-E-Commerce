using Catalog.Application.Abstractions.Persistence.Repositories.Queries;
using MediatR;

namespace Catalog.Application.Product.Queries.BackOffice.GetProductById;

public sealed class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, ProductDetailsResult?>
{
    private readonly IProductReadService _productReadService;

    public GetProductByIdQueryHandler(
        IProductReadService productReadService)
    {
        _productReadService = productReadService;
    }

    public async Task<ProductDetailsResult?> Handle(
        GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        return await _productReadService.GetByIdAsync(
              request.ProductId,
              cancellationToken);
    }
}
