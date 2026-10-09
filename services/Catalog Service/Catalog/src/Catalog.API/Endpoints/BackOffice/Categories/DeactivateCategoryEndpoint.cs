using Catalog.Application.Category.Commands.DeactivateCategory;
using MediatR;

namespace Catalog.API.Endpoints.BackOffice.Categories;

public static class DeactivateCategoryEndpoint
{
    public static IEndpointRouteBuilder MapDeactivateCategoryEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
            "/categories/{id:guid}/deactivate", async (
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
