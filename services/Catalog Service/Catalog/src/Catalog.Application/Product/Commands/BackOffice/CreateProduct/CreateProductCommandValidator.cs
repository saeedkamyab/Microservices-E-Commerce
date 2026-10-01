using FluentValidation;

namespace Catalog.Application.Product.Coammands.BackOffice.CreateProduct;

public class CreateProductCommandValidator:AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x=>x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x=>x.CategoryId)
            .NotEmpty()
            .WithMessage("Category is required.");

        RuleFor(x=>x.Price)
            .NotEmpty()
            .WithMessage("Price is required.");
    }
}
