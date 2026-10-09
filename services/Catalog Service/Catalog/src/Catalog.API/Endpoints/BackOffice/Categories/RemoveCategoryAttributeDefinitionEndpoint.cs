using Catalog.Application.Category.Commands.RemoveAttributeDefinition;
using MediatR;

namespace Catalog.API.Endpoints.BackOffice.Categories
{
    public static class RemoveCategoryAttributeDefinitionEndpoint
    {
        public static IEndpointRouteBuilder MapRemoveCategoryAttributeDefinitionEndpoint(
            this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDelete(
                "/categories/{categoryId:guid}/attributes/{attributeId:guid}",
                async (Guid categoryId, Guid attributeId, ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    await sender.Send(new RemoveAttributeDefinitionCommand(categoryId, attributeId),
                        cancellationToken);
                    return Results.NoContent();
                });
            return endpoints;
        }
    }
}
