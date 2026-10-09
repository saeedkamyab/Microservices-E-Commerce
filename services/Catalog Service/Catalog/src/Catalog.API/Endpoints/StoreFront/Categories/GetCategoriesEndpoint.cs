using Catalog.Application.Category.Queries.BackOffice.GetCategories;
using MediatR;

namespace Catalog.API.Endpoints.StoreFront.Categories;

public static class GetCategoriesEndpoint
{
    public static IEndpointRouteBuilder MapGetCategoriesEndpoint(
       this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
            "/categories",
            async (
                string? search,
                string? status,
                string? sortBy,
                string? sortDirection,
                int? pageNumber,
                int? pageSize,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var query = new GetCategoriesQuery(
                    Search: search,
                    Status: status ,
                    SortBy: sortBy ,
                    SortDirection: sortDirection,
                    PageNumber: pageNumber ?? 1,
                    PageSize: pageSize ?? 20);

                var result = await sender.Send(
                    query,
                    cancellationToken);

                var response = new GetCategoriesResponse(
                    Items: result.Items
                        .Select(x => new CategoryResponse(
                            Id: x.Id,
                            Name: x.Name,
                            Status: x.Status,
                            x.ParentCategoryId,
                            x.ParentCategoryName))
                        .ToArray(),

                    TotalCount: result.TotalCount,
                    PageNumber: result.PageNumber,
                    PageSize: result.PageSize,
                    TotalPages: result.TotalPages,
                    HasPreviousPage: result.HasPreviousPage,
                    HasNextPage: result.HasNextPage);

                return Results.Ok(response);
            });

        return endpoints;
    }

    private sealed record GetCategoriesResponse(
       IReadOnlyCollection<CategoryResponse> Items,
       int TotalCount,
       int PageNumber,
       int PageSize,
       int TotalPages,
       bool HasPreviousPage,
       bool HasNextPage);

    private sealed record CategoryResponse(
        Guid Id,
        string Name,
        string Status,
        Guid? ParentCategoryId,
        string? ParentCategoryName);
}

