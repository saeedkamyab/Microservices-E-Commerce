using Catalog.Application.Category.Queries.BackOffice.GetCategories;

namespace Catalog.UnitTests.Application.Queries.BackOffice.GetCategories;

public class GetCategoriesQueryValidatorTests
{
    private readonly GetCategoriesQueryValidator _validator = new();

    [Fact]
    public void Validate_ValidQuery_ReturnsNoErrors()
    {
        // Arrange
        var query = new GetCategoriesQuery(
            Search: "Test",
            Status: "active",
            SortBy: "name",
            SortDirection: "asc",
            PageNumber: 1,
            PageSize: 10);
        // Act
        var result = _validator.Validate(query);
        // Assert
        Assert.True(result.IsValid);
    }
    [Fact]
    public void Validate_EmptySearch_ReturnsNoErrors()
    {
        // Arrange
        var query = new GetCategoriesQuery(
            Search: "",
            Status: "active",
            SortBy: "name",
            SortDirection: "asc",
            PageNumber: 1,
            PageSize: 10);
        // Act
        var result = _validator.Validate(query);
        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_InvalidStatus_ReturnsError()
    {
        // Arrange
        var query = new GetCategoriesQuery(
            Search: "Test",
            Status: "invalid_status",
            SortBy: "name",
            SortDirection: "asc",
            PageNumber: 1,
            PageSize: 10);
        // Act
        var result = _validator.Validate(query);
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetCategoriesQuery.Status));
    }
    [Fact]
    public void Validate_InvalidSortBy_ReturnsError()
    {
        // Arrange
        var query = new GetCategoriesQuery(
            Search: "Test",
            Status: "active",
            SortBy: "invalid_sort_by",
            SortDirection: "asc",
            PageNumber: 1,
            PageSize: 10);
        // Act
        var result = _validator.Validate(query);
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetCategoriesQuery.SortBy));
    }
    [Fact]
    public void Validate_InvalidSortDirection_ReturnsError()
    {
        // Arrange
        var query = new GetCategoriesQuery(
            Search: "Test",
            Status: "active",
            SortBy: "name",
            SortDirection: "invalid_sort_direction",
            PageNumber: 1,
            PageSize: 10);
        // Act
        var result = _validator.Validate(query);
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetCategoriesQuery.SortDirection));
    }
    [Fact]
    public void Validate_InvalidPageNumber_ReturnsError()
    {
        // Arrange
        var query = new GetCategoriesQuery(
            Search: "Test",
            Status: "active",
            SortBy: "name",
            SortDirection: "asc",
            PageNumber: 0,
            PageSize: 10);
        // Act
        var result = _validator.Validate(query);
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetCategoriesQuery.PageNumber));
    }
    [Fact]
    public void Validate_InvalidPageSize_ReturnsError()
    {
        // Arrange
        var query = new GetCategoriesQuery(
            Search: "Test",
            Status: "active",
            SortBy: "name",
            SortDirection: "asc",
            PageNumber: 1,
            PageSize: 101);
        // Act
        var result = _validator.Validate(query);
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetCategoriesQuery.PageSize));
    }
    

}
