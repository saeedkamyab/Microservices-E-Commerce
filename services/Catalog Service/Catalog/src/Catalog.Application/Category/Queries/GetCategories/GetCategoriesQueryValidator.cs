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
    }
}
