using Lab3.Data;
using Lab3.Domain;

namespace Lab3.Features;

public class CreateBookHandler
{
    private readonly BooksDbContext _context;

    public CreateBookHandler(BooksDbContext context)
    {
        _context = context;
    }

    public async Task<Book> Handle(CreateBookCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Title))
            throw new ValidationException("Title is required.");
        if (string.IsNullOrWhiteSpace(command.Author))
            throw new ValidationException("Author is required.");
        if (command.Year <= 0)
            throw new ValidationException("Year must be a positive number.");

        var book = new Book
        {
            Title = command.Title.Trim(),
            Author = command.Author.Trim(),
            Year = command.Year
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }
}