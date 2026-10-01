using MediatR;

namespace Catalog.Application.Product.Coammands.BackOffice.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string? ProductDescription,
    Guid CategoryId,
    decimal Price
    ) : IRequest<Guid>;



