using Catalog.Application.Product.Coammands.BackOffice.CreateProduct;
using MediatR;

namespace Catalog.API.Endpoints.BackOffice.Products;

public static class CreateProductEndpoint
{
    public static IEndpointRouteBuilder MapCreateProductEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/products", async (
            CreateProductRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {

            var command = new CreateProductCommand(
                request.Name,
                request.ProductDescription,
                request.CategoryId,
                request.Price,
                  request.Specifications
                        .Select(x =>
                            new ProductSpecificationInput(
                                x.AttributeDefinitionId,
                                x.Value))
                        .ToArray());
            var productId = await sender.Send(command, cancellationToken);
            return Results.Created($"/products/{productId}", new { Id = productId });
        });

        return endpoints;
    }
}
public sealed record CreateProductRequest(
    string Name,
    string? ProductDescription,
    Guid CategoryId,
    decimal Price,
    IReadOnlyCollection<CreateProductSpecificationRequest> Specifications
 );

public sealed record CreateProductSpecificationRequest(
    Guid AttributeDefinitionId,
    string Value);