using Inventory.API.Endpoints;
using Inventory.API.Endpoints.InventoryItems;
using Inventory.Application;
using Inventory.Infrastructure;
using Observability;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);


builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource =>
        resource.AddService("Inventory.API"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddSource(MessagingActivitySource.Name)
            .AddOtlpExporter(options =>
            {
                options.Endpoint =
                    new Uri("http://localhost:4317");
            });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapInventoryEndpoints();

app.Run();


