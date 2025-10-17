namespace Lab3.Features;

public record GetAllBooksQuery(string? Author, string? SortBy, int Page = 1, int PageSize = 5);