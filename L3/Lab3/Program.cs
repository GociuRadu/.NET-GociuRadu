using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Lab3.Data;
using Lab3.Features;

var builder = WebApplication.CreateBuilder(args);

// Database path (absolute) and EF Core + SQLite registration
var dbPath = Path.Combine(AppContext.BaseDirectory, "books.db");
builder.Services.AddDbContext<BooksDbContext>(o => o.UseSqlite($"Data Source={dbPath}"));

// CQRS handlers (DI registration)
builder.Services.AddScoped<CreateBookHandler>();
builder.Services.AddScoped<GetAllBooksHandler>();
builder.Services.AddScoped<GetBookByIdHandler>();
builder.Services.AddScoped<UpdateBookHandler>();
builder.Services.AddScoped<DeleteBookHandler>();

var app = builder.Build();

// Global exception handling (maps custom exceptions to HTTP codes)
app.UseMiddleware<ExceptionMiddleware>();

// Apply EF Core migrations at startup (ensures DB and schema exist)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BooksDbContext>();
    db.Database.Migrate();
}

// READ ALL with filtering, sorting, and pagination
app.MapGet("/books", async (string? author, string? sortBy, int? page, int? pageSize, GetAllBooksHandler h) =>
{
    int p = page ?? 1;
    int ps = pageSize ?? 5;
    var books = await h.Handle(new GetAllBooksQuery(author, sortBy, p, ps));
    return Results.Ok(books);
});

// UPDATE existing book by id
app.MapPut("/books/{id}", async (int id, UpdateBookCommand cmd, UpdateBookHandler h) =>
{
    await h.Handle(cmd with { Id = id });
    return Results.NoContent();
});

// DELETE book by id
app.MapDelete("/books/{id}", async (int id, DeleteBookHandler h) =>
{
    await h.Handle(new DeleteBookCommand(id));
    return Results.NoContent();
});

app.Run();