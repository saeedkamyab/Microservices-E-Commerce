using Catalog.API;
using Catalog.API.Exceptions;
using Catalog.Application;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog(
    (context, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration);
    });


var otlpEndpoint =
    builder.Configuration["OpenTelemetry:OtlpEndpoint"]
    ?? "http://localhost:4317";

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource =>
        resource.AddService("Catalog.API"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddNpgsql()
            .AddOtlpExporter(options =>
            {
                options.Endpoint =
                    new Uri(otlpEndpoint);
            });
    })
     .WithMetrics(metrics =>
     {
         metrics
             .AddAspNetCoreInstrumentation()
             .AddHttpClientInstrumentation()
             .AddOtlpExporter(options =>
             {
                 options.Endpoint =
                     new Uri(otlpEndpoint);
             });
     });

     builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.MapCatalogEndpoints();

app.UseExceptionHandler();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<CatalogDbContext>();

    dbContext.Database.Migrate();
}

app.MapHealthChecks("/health");

app.Run();

