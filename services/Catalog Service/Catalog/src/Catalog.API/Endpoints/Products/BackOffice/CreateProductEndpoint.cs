using Catalog.Application.Product.Coammands.BackOffice.CreateProduct;
using MediatR;

namespace Catalog.API.Endpoints.Categories.BackOffice;

public static class CreateProductEndpoint
{
    public static IEndpointRouteBuilder MapCreateProductEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/products", async (
            CreateProductRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {

            var command = new CreateProductCommand(
                request.Name,
                request.ProductDescription,
                request.CategoryId,
                request.Price);
            var productId = await sender.Send(command, cancellationToken);
            return Results.Created($"/api/products/{productId}", new { Id = productId });
        });

        return endpoints;
    }
}
public sealed record CreateProductRequest(
    string Name,
    string? ProductDescription,
    Guid CategoryId,
    decimal Price
 );