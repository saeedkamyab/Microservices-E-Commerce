using Catalog.Application.Category.Coammands.CreateCategory;
using MediatR;

namespace Catalog.API.Endpoints.Categories;

public static class CreateCategoryEndpoint
{
    public static IEndpointRouteBuilder MapCreateCategoryEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/categories",async(
            CreateCategoryRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {

            var command = new CreateCategoryCommand(
                request.Name,
                request.ParentCategoryId);
            var categoryId=await sender.Send(command,cancellationToken);
            return Results.Created($"/api/categories/{categoryId}", new { Id = categoryId });
        });

        return endpoints;
    }
}
public sealed record CreateCategoryRequest(
    string Name,
    Guid? ParentCategoryId
 );