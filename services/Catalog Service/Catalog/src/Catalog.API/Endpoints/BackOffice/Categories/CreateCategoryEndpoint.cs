using Catalog.Application.Category.Commands.CreateCategory;
using MediatR;

namespace Catalog.API.Endpoints.BackOffice.Categories;

public static class CreateCategoryEndpoint
{
    public static IEndpointRouteBuilder MapCreateCategoryEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/categories",async(
            CreateCategoryRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {

            var command = new CreateCategoryCommand(
                request.Name,
                request.ParentCategoryId);
            var categoryId=await sender.Send(command,cancellationToken);
            return Results.Created($"/categories/{categoryId}", new { Id = categoryId });
        });

        return endpoints;
    }
}
public sealed record CreateCategoryRequest(
    string Name,
    Guid? ParentCategoryId
 );