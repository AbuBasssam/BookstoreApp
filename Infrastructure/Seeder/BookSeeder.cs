using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder;

public class BookSeeder
{
    private static readonly Random _rand = new();
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Books.AnyAsync())
            return;

        var languages = await context.Languages.Select(l => l.Id).ToListAsync();
        var categories = await context.Categories.Select(c => c.Id).ToListAsync();
        var publishers = await context.Publishers.Select(p => p.Id).ToListAsync();
        var authors = await context.Authors.Select(a => a.Id).ToListAsync();

        var books = new List<Book>();
        var usersIDsQuery = context.Users.Where(u => u.RoleID.Equals(2)).Select(u => u.Id);


        foreach (var category in categories)
        {
            for (int i = 1; i <= 5; i++)
            {


                // اختيار لغة عشوائية من جدول اللغات
                var langId = languages[_rand.Next(languages.Count)];
                // اختيار ناشر عشوائي من جدول الناشرين
                int publisherId = publishers[_rand.Next(publishers.Count)];
                //أختيار مؤلف عشوائي من جدول المؤلفين
                var authorId = authors[_rand.Next(authors.Count)];

                // توليد Position مطابق للأنماط المطلوبة
                string position = (_rand.Next(2) == 0)
                    ? $"{(char)('A' + _rand.Next(0, 26))}{_rand.Next(10, 100)}"
                    : $"{(char)('A' + _rand.Next(0, 26))}{_rand.Next(10, 100)}-{_rand.Next(0, 10)}{(char)('A' + _rand.Next(0, 26))}";

                var book = new Book
                {
                    ISBN = $"978-{Random.Shared.NextInt64(1000000000, 9999999999)}",
                    TitleEN = $"{category} Book {i}",
                    TitleAR = $"كتاب {i} من {category}",
                    DescriptionEN = $"Description for {category} Book {i}",
                    DescriptionAR = $"وصف الكتاب {i} من {category}",
                    PublisherID = publisherId,
                    AuthorID = authorId,
                    LanguageID = langId,
                    CategoryID = category,
                    PageCount = (short)_rand.Next(100, 600),
                    PublishDate = DateTime.Today.AddYears(-_rand.Next(1, 20)),
                    AvailabilityDate = DateOnly.FromDateTime(DateTime.Today),
                    Position = position,
                    LastWaitListOpenDate = null,
                    IsActive = true,
                    CoverImage = "default_cover.jpg",
                    Copies = Enumerable.Range(1, 5).Select(_ => new BookCopy
                    {
                        IsAvailable = true,
                        IsOnHold = false
                    }).ToList()

                };

                books.Add(book);

            }

        }
        int bookSetCount = 5; // عدد الكتب التي تريد إعطائها تقييمات
        var ratedBooks = books.OrderBy(_ => _rand.Next())
                              .Take(bookSetCount)
                              .ToList();


        foreach (var book in ratedBooks)
        {
            AddBookRatings(book, usersIDsQuery.ToList());
            // AddBookCopies(book, 5);

        }


        await context.Books.AddRangeAsync(books);
        await context.SaveChangesAsync();


    }
    private static void AddBookRatings(Book book, List<int> userIds)
    {

        book.Ratings ??= new List<BookRating>();

        foreach (var userId in userIds)
        {

            // ✅ إضافة تقييمات
            book.Ratings.Add(new BookRating
            {

                UserID = userId,
                Rating = (byte)_rand.Next(1, 6)
            }
            );
        }
    }
    private static void AddBookCopies(Book book, int numberOfCopies)
    {
        book.Copies ??= new List<BookCopy>();
        for (int i = 0; i < numberOfCopies; i++)
        {
            book.Copies.Add(new BookCopy
            {
                IsAvailable = true,
                IsOnHold = false
            });
        }

    }
}
