using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder;

public static class ViewsSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        var connection = context.Database.GetDbConnection();

        await SeederHelper.ExecuteSqlAsync(connection, _GetBooksView());

    }
    private static string _GetBooksView()
    {
        return @"CREATE OR ALTER VIEW vw_Books
AS
SELECT
	b.BookID,
	b.ISBN,
	b.TitleEN,
	b.TitleAR,
	b.DescriptionEN,
	b.DescriptionAR,
	p.PublisherNameEN,
	p.PublisherNameAR,
	a.AuthorNameEN,
	a.AuthorNameAR,
	l.LanguageNameEN AS LanguageEN,
	l.LanguageNameAR AS LanguageAR,
	c.CategoryNameEN,
	c.CategoryNameAR,
	b.PageCount,
	b.PublishDate,
	b.AvailabilityDate,
	b.Position,
	b.LastWaitListOpenDate,
	b.IsActive, 
	b.CoverImage,
	bb.IsBorrowable,
	br.IsReservable,
	r.Rating
	From Books b INNER JOIN Publishers
	p ON b.PublisherID = p.PublisherID INNER JOIN
	Authors a ON b.AuthorID = a.AuthorID INNER JOIN
	Languages l ON b.LanguageID = l.LanguageID INNER JOIN
	Categories c ON b.CategoryID = c.CategoryID
	OUTER APPLY dbo.fn_BookBorrowable(b.BookID) bb
	OUTER APPLY dbo.fn_BookReservable(b.BookID) br
	OUTER APPLY dbo.fn_BookRating(b.BookID) r;";
    }
}
