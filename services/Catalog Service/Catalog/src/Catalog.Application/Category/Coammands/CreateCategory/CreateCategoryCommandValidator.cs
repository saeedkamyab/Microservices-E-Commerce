using FluentValidation;

namespace Catalog.Application.Category.Coammands.CreateCategory;

public class CreateCategoryCommandValidator:AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x=>x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x=>x.Attributes)
            .NotEmpty()
            .WithMessage("Attributes are required.");
    }
}
