using Lab3.Data;
using Lab3.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Features;

public class GetAllBooksHandler
{
    private readonly BooksDbContext _context;
    public GetAllBooksHandler(BooksDbContext context) => _context = context;

    public async Task<List<Book>> Handle(GetAllBooksQuery query)
    {
        if (query.Page < 1) throw new ValidationException("Page must be >= 1.");
        if (query.PageSize < 1 || query.PageSize > 100) throw new ValidationException("PageSize must be between 1 and 100.");

        var q = _context.Books.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Author))
            q = q.Where(b => b.Author.Contains(query.Author));

        q = (query.SortBy?.ToLowerInvariant()) switch
        {
            "title" => q.OrderBy(b => b.Title),
            "year"  => q.OrderBy(b => b.Year),
            _       => q
        };

        var skip = (query.Page - 1) * query.PageSize;
        return await q.Skip(skip).Take(query.PageSize).ToListAsync();
    }
}