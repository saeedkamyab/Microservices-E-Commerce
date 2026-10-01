using FluentValidation;

namespace Catalog.Application.Category.Coammands.AddAttributeDefinition;

public class AddCategoryAttributeDefinitionCommandValidator : AbstractValidator<AddCategoryAttributeDefinitionCommand>
{
    public AddCategoryAttributeDefinitionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("Type is required.");


    }
}
