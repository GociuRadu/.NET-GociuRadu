using Lab3.Data;
using Microsoft.EntityFrameworkCore;
namespace Lab3.Features;


public class DeleteBookHandler
{
    private readonly BooksDbContext _context;
    public DeleteBookHandler(BooksDbContext context) => _context = context;

    public async Task Handle(DeleteBookCommand command)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == command.Id)
                   ?? throw new NotFoundException($"Book with ID {command.Id} not found.");

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }
}