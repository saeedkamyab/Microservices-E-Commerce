namespace Catalog.Application.Common.Enums;

public class SortEnum
{
    public enum CategorySortBy
    {
        Name = 1,
        Status = 2
    }
    public enum ProductSortBy
    {
        Name = 1,
        Status = 2,
        Price = 3
    }

    public enum SortDirection
    {
        Asc = 1,
        Desc = 2
    }
}
