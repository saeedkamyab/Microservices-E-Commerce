using Catalog.Application.Behaviors;
using Catalog.Application.Caching;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
namespace Catalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(
                typeof(DependencyInjection).Assembly);

            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(CachingBehavior<,>));
            config.AddOpenBehavior(typeof(CacheInvalidationBehavior<,>));
        });



        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddSingleton<RedisCircuitBreaker>();

        return services;
    }
}