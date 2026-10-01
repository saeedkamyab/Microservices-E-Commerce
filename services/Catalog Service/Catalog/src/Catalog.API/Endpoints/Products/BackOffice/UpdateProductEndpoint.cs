using Catalog.Application.Product.Coammands.BackOffice.UpdateProduct;
using MediatR;

namespace Catalog.API.Endpoints.Categories.BackOffice;

public static class UpdateProductEndpoint
{

    public static IEndpointRouteBuilder MapUpdateProductEndpoint(
       this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/api/products/{id:guid}", async (
            Guid id,
            UpdateProductRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {

            var command = new UpdateProductCommand(
                id,
                request.Name,
                request.ProductDescription,
                request.CategoryId,
                request.Price);

            await sender.Send(command, cancellationToken);

            return Results.NoContent();
        });

        return endpoints;
    }

}


public sealed record UpdateProductRequest
{
    public string Name { get; set; } = null!;
    public string? ProductDescription { get; set; }
    public Guid CategoryId { get; set; }
    public decimal Price { get; set; }

}