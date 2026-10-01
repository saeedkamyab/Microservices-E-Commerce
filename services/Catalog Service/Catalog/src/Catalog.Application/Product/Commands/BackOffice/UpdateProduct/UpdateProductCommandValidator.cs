using FluentValidation;

namespace Catalog.Application.Product.Coammands.BackOffice.UpdateProduct;

public class UpdateProductCommandValidator:AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category is required.");

        RuleFor(x => x.Price)
            .NotEmpty()
            .WithMessage("Price is required.");
    }
}
