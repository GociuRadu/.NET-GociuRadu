using AutoMapper;
using FluentValidation;
using Labb5.DTOs;
using Labb5.Mapping;
using Labb5.Middleware;
using Labb5.Products;
using Labb5.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddMemoryCache();
builder.Services.AddAutoMapper(typeof(AdvancedProductMappingProfile).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductProfileValidator>();
builder.Services.AddSingleton<CreateProductHandler>();

var app = builder.Build();

app.UseMiddleware<CorrelationMiddleware>();

app.MapPost("/products",
    async (Labb5.DTOs.CreateProductProfileRequest request,
        Labb5.Products.CreateProductHandler handler,
        FluentValidation.IValidator<Labb5.DTOs.CreateProductProfileRequest> validator) =>
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return Results.BadRequest(new {
                message = "Validation failed",
                errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });

        try
        {
            var result = handler.Handle(request);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    });

app.MapGet("/products",
    (Labb5.Products.CreateProductHandler handler) =>
    {
        var products = handler.GetAll();
        return Results.Ok(products);
    });

app.MapGet("/", () => Results.Ok("OK"));

app.Run();