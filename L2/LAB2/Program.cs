using System;
using System.Collections.Generic;
using LAB2;
//Function to see the type of the object using pattern matching
static void ObjectType(object obj)
{
   
    if (obj is Book { Title: var t, Author: var a, YearPublished: var y })
    {
        Console.WriteLine($"Book: {t} ({y})");
    }
    else if (obj is Borrower { Name: var n, BorrowedBooks: var list })
    {
        Console.WriteLine($"Borrower: {n} - {list.Count} carti imprumutate");
    }
    else
    {
        Console.WriteLine("Unknown type");
    }
}


var b1 = new Book("1984", "George Orwell", 1949);
var b2 = new Book("To Kill a Mockingbird", "Harper Lee", 1960);
var b3 = new Book("The Great Gatsby", "F. Scott Fitzgerald", 1925);
var b4 = new Book("Enigma", "Calinescu", 2020);

var borrower1 = new Borrower(1, "Alice", new List<Book> { b1, b2 });
var borrower2 = borrower1 with
{
    BorrowedBooks = new List<Book>(borrower1.BorrowedBooks) { b4 }
};

Console.WriteLine($"{borrower1.Name} -> {borrower1.BorrowedBooks.Count} carti");
Console.WriteLine($"{borrower2.Name} -> {borrower2.BorrowedBooks.Count} carti ");

var librarian = new Librarian
{
    Name = "John Doe",
    Email = "jhon@libray.ro",
    LibrarySection = "Fiction",
};

Console.WriteLine($"Librarian: {librarian.Name}, Email: {librarian.Email}, Section: {librarian.LibrarySection}");

//Reading book titles from console until an empty line is entered
var books=new List<string>();
Console.WriteLine("Introduceti cartile:");

//loop until an empty line is entered
while (true)
{
    var title=Console.ReadLine();
    if (string.IsNullOrEmpty(title))
        break;
    
    books.Add(title);
}

Console.WriteLine("Cartile introduse sunt:");
foreach(var book in books)
    Console.WriteLine(book);

ObjectType(b1);
ObjectType(borrower1);
ObjectType(100);

//filter books published after 2010
var allBooks = new List<Book> { b1, b2, b3, b4 };

var recentBooks = allBooks.Where(static b => b.YearPublished >= 2010);

int cnt = 0;
foreach(var b in recentBooks)
{
    Console.WriteLine($"{++cnt}. {b.Title} ({b.YearPublished})");
}