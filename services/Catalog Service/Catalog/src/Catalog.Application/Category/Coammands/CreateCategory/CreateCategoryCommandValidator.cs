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
        
        RuleForEach(x => x.Attributes)
           .ChildRules(attribute =>
           {
               attribute.RuleFor(x => x.Name)
                   .NotEmpty()
                   .WithMessage("Attribute name is required.");

               attribute.RuleFor(x => x.Options)
                   .NotNull()
                   .WithMessage("Attribute options cannot be null.");
           });
    }
}
