using Catalog.Application.Product.Coammands.BackOffice.UpdateProduct;
using MediatR;

namespace Catalog.API.Endpoints.BackOffice.Products;

public static class UpdateProductEndpoint
{

    public static IEndpointRouteBuilder MapUpdateProductEndpoint(
       this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/products/{id:guid}", async (
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
                request.Price,
                  request.Specifications
                   .Select(x =>
                            new ProductSpecificationInput(
                                x.AttributeDefinitionId,
                                x.Value))
                        .ToArray());

            await sender.Send(command, cancellationToken);

            return Results.NoContent();
        });

        return endpoints;
    }

}


public sealed record UpdateProductRequest(
    string Name,
    string? ProductDescription,
    Guid CategoryId,
    decimal Price,
    IReadOnlyCollection<UpdateProductSpecificationRequest> Specifications
 );

public sealed record UpdateProductSpecificationRequest(
    Guid AttributeDefinitionId,
    string Value);