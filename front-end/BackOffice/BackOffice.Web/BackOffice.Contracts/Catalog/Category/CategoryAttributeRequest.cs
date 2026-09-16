namespace BackOffice.Contracts.Catalog.Category;

public sealed class CategoryAttributeRequest
{
    public string Name { get; set; } = string.Empty;

    public int Type { get; set; } = 1;

    public bool IsRequired { get; set; }

    public List<string> Options { get; set; } = [];
}

