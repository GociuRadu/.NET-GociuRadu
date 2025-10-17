using Microsoft.EntityFrameworkCore;
using Lab3.Domain;

namespace Lab3.Data;

public class BooksDbContext : DbContext
{
    public BooksDbContext(DbContextOptions<BooksDbContext> options)
        : base(options) { }

    public DbSet<Book> Books => Set<Book>();
}