using FluentValidation;

namespace Catalog.Application.Category.Coammands.UpdateCategory;

public class UpdateCategoryCommandValidator:AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x=>x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");
    }
}
