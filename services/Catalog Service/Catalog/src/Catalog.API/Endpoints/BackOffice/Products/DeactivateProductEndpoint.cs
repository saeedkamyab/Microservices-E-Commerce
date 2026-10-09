using Catalog.Application.Product.Coammands.BackOffice.DeactivateProduct;
using MediatR;

namespace Catalog.API.Endpoints.BackOffice.Products;

public static class DeactivateProductEndpoint
{
    public static IEndpointRouteBuilder MapDeactivateProductEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
            "/products/{id:guid}/deactivate", async (
            Guid id, ISender sender,
            CancellationToken cancellationToken) =>
            {
                await sender.Send(new DeactivateProductCommand(id),
                    cancellationToken);
                return Results.NoContent();
            });
        return endpoints;
    }
}
