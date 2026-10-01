using Catalog.Application.Category.Coammands.AddAttributeDefinition;
using MediatR;

namespace Catalog.API.Endpoints.Categories.BackOffice;

public static class AddCategoryAttributeDefinitionEndpoint
{
    public static IEndpointRouteBuilder MapAddCategoryAttributeDefinitionEndpoint(
    this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/api/categories/{id:guid}/attribute-definitions", async (
            Guid id,
      AddCategoryAttributeDefinitionRequest request,
      ISender sender,
      CancellationToken cancellationToken) =>
        {

            var command = new AddCategoryAttributeDefinitionCommand(
                CategoryId: id,
               Name: request.Name,
               Type: request.Type,
               IsRequired: request.IsRequired,
               Options: request.Options);
            var attributeId = await sender.Send(command, cancellationToken);

            return Results.Created($"/api/categories/{id}/attribute-definitions/{attributeId}", new { Id = attributeId });
           
        });

        return endpoints;
    }


}

public sealed record AddCategoryAttributeDefinitionRequest
{
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public bool IsRequired { get; init; }
    public IReadOnlyCollection<string> Options { get; init; } = Array.Empty<string>();
}