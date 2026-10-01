using Catalog.Application.Abstractions.Persistence.Repositories.Queries;
using Catalog.Application.Common;
using Catalog.Application.Common.Enums;
using MediatR;
using static Catalog.Application.Common.Enums.SortEnum;

namespace Catalog.Application.Product.Queries.BackOffice.GetProducts;

public sealed class GetProductsQueryHandler
       : IRequestHandler<
        GetProductsQuery,
        PagedResult<ProductListItem>>
{
    private readonly IProductReadService _categoryReadService;

    public GetProductsQueryHandler(
        IProductReadService categoryReadService)
    {
        _categoryReadService = categoryReadService;
    }

    public Task<PagedResult<ProductListItem>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        return _categoryReadService.GetProductsAsync(
            request.Search,
            StringToEnumConvertor.ToNullableEnum<ProductStatusFilter>(request.Status),
            StringToEnumConvertor.ToEnum<ProductSortBy>(request.SortBy),
            StringToEnumConvertor.ToEnum<SortDirection>(request.SortDirection),
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
