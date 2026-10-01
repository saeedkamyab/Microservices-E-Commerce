using MediatR;

namespace Catalog.Application.Product.Queries.BackOffice.GetProductById;

public sealed record GetProductByIdQuery(
    Guid ProductId
) : IRequest<ProductDetailsResult?>;