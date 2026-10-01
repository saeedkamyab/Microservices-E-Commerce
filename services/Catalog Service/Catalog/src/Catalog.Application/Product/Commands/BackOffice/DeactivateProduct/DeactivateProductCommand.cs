using MediatR;

namespace Catalog.Application.Product.Coammands.BackOffice.DeactivateProduct;

public sealed record DeactivateProductCommand(Guid ProductId) : IRequest;

