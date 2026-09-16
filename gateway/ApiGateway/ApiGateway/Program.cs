using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseSerilog(
    (context, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration);
    });


builder.Services.AddReverseProxy()
    .LoadFromConfig(
      builder.Configuration.GetSection("ReverseProxy")
    );

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource =>
    resource.AddService("ApiGateway"))
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter();
    });
    



var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.MapReverseProxy();


app.Run();

