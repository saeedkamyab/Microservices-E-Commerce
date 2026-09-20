using Catalog.Application.Category.Coammands.DeactivateCategory;
using MediatR;

namespace Catalog.API.Endpoints.Categories;

public static class DeactivateCategoryEndpoint
{
    public static IEndpointRouteBuilder MapDeactivateCategoryEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
            "/api/categories/{id:guid}/deactivate", async (
            Guid id,ISender sender,
            CancellationToken cancellationToken) =>
            {
                await sender.Send(new DeactivateCategoryCommand(id),
                    cancellationToken);
                return Results.NoContent();
            });
        return endpoints;
    }
}
