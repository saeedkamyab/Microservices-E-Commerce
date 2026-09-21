using Catalog.Application.Category.Coammands.UpdateCategory;
using MediatR;

namespace Catalog.API.Endpoints.Categories;

public static class UpdateCategoryEndpoint
{

    public static IEndpointRouteBuilder MapUpdateCategoryEndpoint(
       this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/api/categories/{id:guid}", async (
            Guid id,
            UpdateCategoryRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {

            var command = new UpdateCategoryCommand(
                id,
                request.Name,
                request.ParentCategoryId);

            await sender.Send(command, cancellationToken);

            return Results.NoContent();
        });

        return endpoints;
    }

}


public sealed record UpdateCategoryRequest
{
    public string Name { get; set; } = null!;
    public Guid? ParentCategoryId { get; set; }
}