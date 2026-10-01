using MediatR;

namespace Catalog.Application.Product.Coammands.BackOffice.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid ProductId,
    string Name,
    string? ProductDescription,
    Guid CategoryId,
    decimal Price
    ) : IRequest;

