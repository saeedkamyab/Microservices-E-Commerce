using Catalog.Application.Product.Coammands.BackOffice.ActivateProduct;
using MediatR;

namespace Catalog.API.Endpoints.BackOffice.Products;

public static class ActivateProductEndpoint
{
    public static IEndpointRouteBuilder MapActivateProductEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
            "/products/{id:guid}/activate", async (
            Guid id, ISender sender,
            CancellationToken cancellationToken) =>
            {
                await sender.Send(new ActivateProductCommand(id),
                    cancellationToken);
                return Results.NoContent();
            });
        return endpoints;
    }
}
