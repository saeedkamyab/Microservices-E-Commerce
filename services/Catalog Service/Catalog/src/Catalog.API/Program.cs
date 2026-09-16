using Catalog.API;
using Catalog.API.Exceptions;
using Catalog.Application;
using Catalog.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.MapCatalogEndpoints();

app.UseExceptionHandler();

app.Run();

