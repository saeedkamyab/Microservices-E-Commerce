using Catalog.Application.Product.Queries.BackOffice.GetProductById;
using MediatR;

namespace Catalog.API.Endpoints.StoreFront.Products;

public static class GetProductByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetProductByIdEndpoint(
        this IEndpointRouteBuilder endpoints)
    {

        endpoints.MapGet(
     "/products/{id:guid}",
     async (
         Guid id,
         ISender sender,
         CancellationToken cancellationToken) =>
     {
         var result = await sender.Send(
             new GetProductByIdQuery(id),
             cancellationToken);

         if (result is null)
         {
             return Results.NotFound();
         }

         var response = new GetProductByIdResponse(
             Id: result.Id,
             Name: result.Name,
             ProductDescription: result.ProductDescription,
             CategoryId: result.CategoryId,
             Price: result.Price,
             Status: result.Status,
             Specifications: result.Specifications
                 .Select(x => new ProductSpecificationResponse(
                     AttributeDefinitionId: x.AttributeDefinitionId,
                     Value: x.Value))
                 .ToArray());

         return Results.Ok(response);
     });

        return endpoints;
    }

}


public sealed record GetProductByIdResponse(
    Guid Id,
    string Name,
    string? ProductDescription,
    Guid CategoryId,
    decimal Price,
    string Status,
    IReadOnlyCollection<ProductSpecificationResponse> Specifications);

public sealed record ProductSpecificationResponse(
    Guid AttributeDefinitionId,
    string Value);