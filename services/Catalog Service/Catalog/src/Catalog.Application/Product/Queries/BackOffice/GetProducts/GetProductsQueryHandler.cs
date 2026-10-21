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
    private readonly IProductReadService _productReadService;
  
    public GetProductsQueryHandler(
        IProductReadService productReadService)
    {
        _productReadService = productReadService;
      
    }

    public async Task<PagedResult<ProductListItem>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var status = StringToEnumConvertor
            .ToNullableEnum<ProductStatusFilter>(request.Status);


        var sortBy = StringToEnumConvertor
            .ToEnum<ProductSortBy>(request.SortBy);

        var sortDirection = StringToEnumConvertor
            .ToEnum<SortDirection>(request.SortDirection);


        var result = await _productReadService.GetProductsAsync(
            request.Search,
            status,
            sortBy,
            sortDirection,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return result;
    }

  
}
