using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Catalog.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            ValidationResult[] validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults.SelectMany(v => v.Errors)
                .Where(f => f != null)
                .ToList();
            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }
            return await next();
    }
}
