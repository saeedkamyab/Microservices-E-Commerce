namespace BackOffice.Components.Components.Category.Models;

public sealed class CategoryFormModel
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? ParentCategoryId { get; set; }
}
