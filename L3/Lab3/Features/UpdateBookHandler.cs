namespace Lab3.Features;
using Microsoft.EntityFrameworkCore;
using Lab3.Data;

public class UpdateBookHandler
{
    private readonly BooksDbContext _context;
    public UpdateBookHandler(BooksDbContext context) => _context = context;

    public async Task Handle(UpdateBookCommand command)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == command.Id)
                   ?? throw new NotFoundException($"Book with ID {command.Id} not found.");

        if (string.IsNullOrWhiteSpace(command.Title))
            throw new ValidationException("Title is required.");
        if (string.IsNullOrWhiteSpace(command.Author))
            throw new ValidationException("Author is required.");

        book.Title = command.Title.Trim();
        book.Author = command.Author.Trim();
        book.Year = command.Year;

        await _context.SaveChangesAsync();
    }
}