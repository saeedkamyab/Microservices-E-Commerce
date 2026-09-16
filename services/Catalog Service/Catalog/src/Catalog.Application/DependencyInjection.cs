using Catalog.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
namespace Catalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(config =>
           config.RegisterServicesFromAssembly(
               typeof(DependencyInjection).Assembly));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddTransient(
typeof(IPipelineBehavior<,>),
typeof(ValidationBehavior<,>));

        return services;
    }
}