using Catalog.Application.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Exceptions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case ValidationException validationException:
                await HandleValidationExceptionAsync(
                    httpContext,
                    validationException,
                    cancellationToken);

                return true;

            case NotFoundException:
                await WriteProblemDetailsAsync(
                    httpContext,
                    exception,
                    StatusCodes.Status404NotFound,
                    "Resource not found.",
                    cancellationToken);

                return true;

            case ConflictException:
                await WriteProblemDetailsAsync(
                    httpContext,
                    exception,
                    StatusCodes.Status409Conflict,
                    "Conflict.",
                    cancellationToken);

                return true;

            case ArgumentException:
                await WriteProblemDetailsAsync(
                    httpContext,
                    exception,
                    StatusCodes.Status400BadRequest,
                    "Invalid request.",
                    cancellationToken);

                return true;

            default:
                _logger.LogError(
                    exception,
                    "An unhandled exception occurred while processing the request.");

                await WriteProblemDetailsAsync(
                    httpContext,
                    exception,
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred.",
                    cancellationToken,
                    includeExceptionMessage: false);

                return true;
        }
    }

    private static async Task HandleValidationExceptionAsync(
        HttpContext context,
        ValidationException exception,
        CancellationToken cancellationToken)
    {
        var errors = exception.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                x => x.Key,
                x => x.Select(e => e.ErrorMessage).ToArray());

        var problemDetails =
            new HttpValidationProblemDetails(errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation failed.",
                Detail = "One or more validation errors occurred."
            };

        context.Response.StatusCode =
            StatusCodes.Status400BadRequest;

        await context.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);
    }

    private static async Task WriteProblemDetailsAsync(
        HttpContext context,
        Exception exception,
        int statusCode,
        string title,
        CancellationToken cancellationToken,
        bool includeExceptionMessage = true)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = includeExceptionMessage
                ? exception.Message
                : "An unexpected server error occurred."
        };

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);
    }
}