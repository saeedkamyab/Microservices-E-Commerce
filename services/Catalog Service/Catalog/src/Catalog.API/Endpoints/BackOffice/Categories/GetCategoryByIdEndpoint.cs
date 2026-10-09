using Catalog.Application.Category.Queries.BackOffice.GetCategoryById;
using MediatR;

namespace Catalog.API.Endpoints.BackOffice.Categories;

public static class GetCategoryByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetCategoryByIdEndpoint(
        this IEndpointRouteBuilder endpoints)
    {

        endpoints.MapGet(
     "/categories/{id:guid}",
     async (
         Guid id,
         ISender sender,
         CancellationToken cancellationToken) =>
     {
         var result = await sender.Send(
             new GetCategoryByIdQuery(id),
             cancellationToken);

         if (result is null)
         {
             return Results.NotFound();
         }

         var response = new GetCategoryByIdResponse(
             result.Id,
             result.Name,
             result.ParentCategoryId,
             result.Status,
             result.Attributes
                 .Select(x => new CategoryAttributeResponse(
                     x.Id,
                     x.Name,
                     x.Type,
                     x.IsRequired,
                     x.Options))
                 .ToArray());

         return Results.Ok(response);
     });

        return endpoints;

    }

}


public sealed record GetCategoryByIdResponse(
    Guid Id,
    string Name,
    Guid? ParentCategoryId,
    string Status,
    IReadOnlyCollection<CategoryAttributeResponse> Attributes);

public sealed record CategoryAttributeResponse(
    Guid Id,
    string Name,
    string Type,
    bool IsRequired,
    IReadOnlyCollection<string> Options);