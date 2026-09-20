using FluentValidation;

namespace Catalog.Application.Category.Queries.GetCategories;

public sealed class GetCategoriesQueryValidator
    : AbstractValidator<GetCategoriesQuery>
{
    public GetCategoriesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.Search)
       .MaximumLength(100)
       .When(x => !string.IsNullOrWhiteSpace(x.Search))
       .WithMessage("Search must not exceed 100 characters.");

        RuleFor(x => x.Status)
         .Must(status =>
             string.IsNullOrWhiteSpace(status) ||
             status.Equals("active", StringComparison.OrdinalIgnoreCase) ||
             status.Equals("inactive", StringComparison.OrdinalIgnoreCase))
         .WithMessage("Status must be either 'active' or 'inactive'.");

        RuleFor(x => x.SortBy)
            .Must(sortBy =>
                string.IsNullOrWhiteSpace(sortBy) ||
                sortBy.Equals("name", StringComparison.OrdinalIgnoreCase) ||
                sortBy.Equals("status", StringComparison.OrdinalIgnoreCase))
            .WithMessage("SortBy must be either 'name' or 'status'.");

        RuleFor(x => x.SortDirection)
            .Must(direction =>
                string.IsNullOrWhiteSpace(direction) ||
                direction.Equals("asc", StringComparison.OrdinalIgnoreCase) ||
                direction.Equals("desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("SortDirection must be either 'asc' or 'desc'.");

    }
}
