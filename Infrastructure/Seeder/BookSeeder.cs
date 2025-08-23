using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder;

public class BookSeeder
{
    private static readonly Random _rand = new();
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Books.AnyAsync())
            return;

        var languages = await context.Languages.ToListAsync();

        var books = new List<Book>();


        foreach (enCategory category in Enum.GetValues(typeof(enCategory)))
        {
            for (int i = 1; i <= 5; i++)
            {
                // إنشاء مؤلف جديد
                var author = new Author
                {
                    NameEN = $"Author {category} {i}",
                    NameAR = $"مؤلف {category} {i}",
                    BirthDate = null,
                    Bio = $"Author for {category} book "
                };
                await context.Authors.AddAsync(author);
                await context.SaveChangesAsync();


                // إنشاء دار نشر جديدة
                var publisher = new Publisher
                {
                    NameEN = $"Publisher {category} {i}",
                    NameAR = $"دار نشر {category} {i}"
                };
                await context.Publishers.AddAsync(publisher);
                await context.SaveChangesAsync();


                // اختيار لغة عشوائية من جدول اللغات
                var lang = languages[_rand.Next(languages.Count)];

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
                    PublisherID = publisher.Id,
                    AuthorID = author.Id,
                    LanguageID = lang.Id,
                    CategoryID = category,
                    PageCount = (short)_rand.Next(100, 600),
                    PublishDate = DateTime.Today.AddYears(-_rand.Next(1, 20)),
                    AvailabilityDate = DateOnly.FromDateTime(DateTime.Today),
                    Position = position,
                    LastWaitListOpenDate = null,
                    IsActive = true,
                    CoverImage = "default_cover.jpg",

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
            AddBookRatings(book);
            AddBookCopies(book, 5);

        }


        await context.Books.AddRangeAsync(books);
        await context.SaveChangesAsync();


    }
    private static void AddBookRatings(Book book)
    {
        var userIds = new List<int> { 1009, 1010, 1011, 1012, 1013 };
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
