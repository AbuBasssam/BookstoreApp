using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infrastructure.Seeder;

public class BookSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (!context.Books.Any())
        {
            var categories = await SeedCategories(context);

            var authors = await SeedAuthors(context);
            var books = new List<Book>();

            foreach (var category in categories)
            {
                for (int i = 1; i <= 5; i++)
                {
                    var book = new Book
                    {
                        TitleEN = $"{category.NameEN} Book {i}",
                        TitleAR = $"كتاب {category.NameAR} {i}",
                        PublishDate = DateTime.Now.AddYears(-i),
                        ISBN = GenerateISBN(Enum.Parse<enCategory>(category.NameEN), i),
                        CategoryID = category.Id,
                        AuthorID = authors[(i - 1) % authors.Length].Id,
                        CoverImage = $"/images/books/{category.NameEN.ToLower()}_{i}.jpg",
                        Copies = new List<BookCopy>()
                    };

                    //Add 5 copies for each book
                    for (int j = 1; j <= 5; j++)
                    {
                        book.Copies.Add(new BookCopy());
                    }
                    books.Add(book);


                }
            }
            await context.Books.AddRangeAsync(books);
            await context.SaveChangesAsync();

            //await context.SaveChangesAsync();
        }
    }

    private static async Task<Author[]> SeedAuthors(AppDbContext context)
    {
        if (!context.Author.Any())
        {
            var authors = new[]
            {
                    new Author
                    {
                        AuthorNameEN = "John Smith",
                        AuthorNameAR = "جون سميث",
                        BirthDate = new DateTime(1970, 1, 1),
                        Bio = "Renowned author with multiple bestsellers"
                    },
                    new Author
                    {
                        AuthorNameEN = "Sarah Johnson",
                        AuthorNameAR = "سارة جونسون",
                        BirthDate = new DateTime(1980, 5, 15),
                        Bio = "Award-winning novelist"
                    },
                    new Author
                    {
                        AuthorNameEN = "Michael Brown",
                        AuthorNameAR = "مايكل براون",
                        BirthDate = new DateTime(1965, 11, 22),
                        Bio = "Prolific writer with 30 published works"
                    },
                    new Author
                    {
                        AuthorNameEN = "Emily Davis",
                        AuthorNameAR = "إيميلي ديفيس",
                        BirthDate = new DateTime(1975, 3, 8),
                        Bio = "Emerging voice in contemporary literature"
                    },
                    new Author
                    {
                        AuthorNameEN = "Robert Wilson",
                        AuthorNameAR = "روبرت ويلسون",
                        BirthDate = new DateTime(1958, 7, 30),
                        Bio = "Master of historical fiction"
                    }
                };

            await context.Author.AddRangeAsync(authors);
            await context.SaveChangesAsync();
            return authors;
        }

        return await context.Author.Take(5).ToArrayAsync();
    }

    private static async Task<Category[]> SeedCategories(AppDbContext context)
    {
        if (!context.Categories.Any())
        {
            var categories = Enum.GetValues(typeof(enCategory)).Cast<enCategory>()
                .Select(c => new Category
                {
                    Id = c,
                    NameEN = c.ToString(),
                    NameAR = GetCategoryNameAR(c)
                }).ToList();
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }
        return await context.Categories.ToArrayAsync();
    }

    private static string GenerateISBN(enCategory category, int index)
    {
        var prefix = (int)category * 1000 + index;
        var middle = DateTime.Now.Year;
        var suffix = new Random().Next(1000, 9999);
        return $"{prefix}-{middle}-{suffix}";
    }

    private static string GetCategoryNameAR(enCategory category)
    {
        return category switch
        {
            enCategory.Romance => "رومانسي",
            enCategory.Fantasy => "خيال",
            enCategory.Mystery => "غموض",
            enCategory.Programming => "برمجة",
            enCategory.Economics => "اقتصاد",
            enCategory.History => "تاريخ",
            enCategory.Science => "علوم",
            _ => "عام"
        };
    }
}
