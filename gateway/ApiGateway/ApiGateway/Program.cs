using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddReverseProxy()
    .LoadFromConfig(
      builder.Configuration.GetSection("ReverseProxy")
    );

builder.Host.UseSerilog(
    (context, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.MapReverseProxy();


app.Run();

