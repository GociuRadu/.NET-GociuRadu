using Lab3.Data;
using Lab3.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Features;
public class GetBookByIdHandler
{
    private readonly BooksDbContext _context;
    public GetBookByIdHandler(BooksDbContext context) => _context = context;

    public async Task<Book> Handle(GetBookByIdQuery query)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == query.Id);
        if (book == null)
            throw new NotFoundException($"Book with ID {query.Id} not found.");

        return book;
    }
}