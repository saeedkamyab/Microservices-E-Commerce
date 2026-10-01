using MediatR;

namespace Catalog.Application.Product.Coammands.BackOffice.ActivateProduct;

public sealed record ActivateProductCommand(Guid ProductId) : IRequest;

