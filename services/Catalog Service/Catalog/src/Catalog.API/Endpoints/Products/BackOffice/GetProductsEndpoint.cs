using Catalog.Application.Product.Queries.BackOffice.GetProducts;
using MediatR;

namespace Catalog.API.Endpoints.Products.BackOffice;

public static class GetProductsEndpoint
{
    public static IEndpointRouteBuilder MapGetProductsEndpoint(
       this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
            "/api/products",
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
                var query = new GetProductsQuery(
                    Search: search,
                    Status: status,
                    SortBy: sortBy,
                    SortDirection: sortDirection,
                    PageNumber: pageNumber ?? 1,
                    PageSize: pageSize ?? 20);

                var result = await sender.Send(
                    query,
                    cancellationToken);

                var response = new GetProductsResponse(
                    Items: result.Items
                        .Select(x => new ProductResponse(
                            Id: x.Id,
                            Name: x.Name,
                            ProductDescription: x.ProductDescription,
                            CategoryId: x.CategoryId,
                            Status: x.Status,
                            Price: x.Price,
                            CategoryName: x.CategoryName))
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

    private sealed record GetProductsResponse(
       IReadOnlyCollection<ProductResponse> Items,
       int TotalCount,
       int PageNumber,
       int PageSize,
       int TotalPages,
       bool HasPreviousPage,
       bool HasNextPage);

    private sealed record ProductResponse(
        Guid Id,
        string Name,
        string? ProductDescription,
        Guid CategoryId,
        string Status,
        decimal Price,
        string CategoryName);
}

