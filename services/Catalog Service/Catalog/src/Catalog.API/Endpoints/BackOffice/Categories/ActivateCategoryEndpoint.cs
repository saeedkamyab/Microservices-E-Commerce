using Catalog.Application.Category.Commands.ActivateCategory;
using MediatR;

namespace Catalog.API.Endpoints.BackOffice.Categories;

public static class ActivateCategoryEndpoint
{
    public static IEndpointRouteBuilder MapActivateCategoryEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
            "/categories/{id:guid}/activate", async (
            Guid id, ISender sender,
            CancellationToken cancellationToken) =>
            {
                await sender.Send(new ActivateCategoryCommand(id),
                    cancellationToken);
                return Results.NoContent();
            });
        return endpoints;
    }
}
