using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder;
public class SPAndFunctionsSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {

        var connection = context.Database.GetDbConnection();

        await connection.OpenAsync();

        await SeederHelper.ExecuteSqlAsync(connection, _GetBookReservableFunction());
        await SeederHelper.ExecuteSqlAsync(connection, _GetBookBorrowableFunction());
        await SeederHelper.ExecuteSqlAsync(connection, _GetBookRatingFunction());
        await SeederHelper.ExecuteSqlAsync(connection, _SelectByLanguageFunction());
        await SeederHelper.ExecuteSqlAsync(connection, _NewestBooksFunction());
        await SeederHelper.ExecuteSqlAsync(connection, _MostRecentBooksFunction());
        await SeederHelper.ExecuteSqlAsync(connection, _FirstCategoryBooksFunction());
        await SeederHelper.ExecuteSqlAsync(connection, _SP_GetHomePageData());
    }
    private static string _GetBookBorrowableFunction()
    {
        return @"
CREATE OR ALTER FUNCTION dbo.fn_BookBorrowable(@BookID INT)
RETURNS TABLE
AS
RETURN
(
    SELECT TOP 1
        CASE 
            WHEN EXISTS (
                SELECT 1
                FROM BookCopies bc
                WHERE bc.BookID = @BookID
                  AND bc.IsAvailable = 1
                  AND bc.IsOnHold = 0
            )
            THEN CAST(1 AS BIT)
            ELSE CAST(0 AS BIT)
        END AS IsBorrowable
);";
    }
    private static string _GetBookReservableFunction()
    {
        return @"
CREATE OR ALTER FUNCTION dbo.fn_BookReservable(@BookID INT)
RETURNS TABLE
AS
RETURN
(
    SELECT TOP 1
        CASE 
            WHEN EXISTS (
                SELECT 1 
                FROM BookCopies bc
                WHERE bc.BookID = @BookID
                  AND bc.IsAvailable = 0
                  AND bc.IsOnHold = 0
            )
            THEN CAST(1 AS BIT)
            ELSE CAST(0 AS BIT)
        END AS IsReservable
);";
    }
    private static string _GetBookRatingFunction()
    {
        return @"
CREATE OR ALTER FUNCTION dbo.fn_BookRating(@BookID INT)
RETURNS TABLE
AS
RETURN
(
    SELECT TOP 1
        AVG(CAST(Rating AS DECIMAL(3,2))) AS Rating
    FROM BookRatings br
    WHERE br.BookID = @BookID
    GROUP BY br.BookID
);";
    }
    private static string _SelectByLanguageFunction()
    {
        return @"
CREATE OR ALTER FUNCTION dbo.fn_SelectByLanguage( @Lang NVARCHAR(2),
    @ValueEN NVARCHAR(MAX),
    @ValueAR NVARCHAR(MAX)
)
RETURNS TABLE
AS
RETURN
(
    SELECT CASE 
             WHEN @Lang = 'ar' THEN @ValueAR 
             ELSE @ValueEN 
           END AS Value
);";
    }
    private static string _NewestBooksFunction()
    {
        return @"
CREATE OR ALTER FUNCTION dbo.fn_NewestBooks( @NewBooksDaysThreshold INT,@PageNumber INT,@PageSize INT)
RETURNS TABLE
AS
RETURN
(
    SELECT b.BookID, CAST(1 AS BIT) AS IsNewBook 
    FROM Books b
    WHERE b.IsActive = 1
      AND b.AvailabilityDate >= DATEADD(DAY, -@NewBooksDaysThreshold, GETDATE()) 
ORDER BY 
        b.AvailabilityDate DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY
);";
    }
    private static string _MostRecentBooksFunction()
    {
        return @"
CREATE OR ALTER FUNCTION dbo.fn_MostRecentBooks( @PopularityDaysThreshold INT, @MostPopularBooksCount INT)
RETURNS TABLE
AS
RETURN
(
SELECT TOP (@MostPopularBooksCount)  bc.BookID 
                FROM BorrowingRecords br INNER join BookCopies bc 
				on br.BookCopyID=bc.CopyID
				WHERE br.BorrowingDate >= DATEADD(DAY, -@PopularityDaysThreshold, GETDATE())
                GROUP BY bc.BookID 
                ORDER BY COUNT(*) DESC
    );";
    }
    private static string _FirstCategoryBooksFunction()
    {
        return @"
CREATE OR ALTER FUNCTION dbo.fn_FirstCategoryBooks(@PageNumber INT,@PageSize INT)
RETURNS TABLE
AS
RETURN
(
    SELECT b.BookID, CAST(1 AS BIT) AS IsFirstCategory 
    FROM Books b
    WHERE b.IsActive = 1
      AND b.CategoryID=1
ORDER BY 
        b.BookID DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY
);";
    }
    private static string _SP_GetHomePageData()
    {
        return @"
CREATE PROCEDURE SP_GetHomePageData
@NewBooksDaysThreshold INT,
@PopularityDaysThreshold INT,
@PopularBooksCount INT,
@Lang CHAR(2)
AS
BEGIN
SET NOCOUNT ON;
SELECT
c.CategoryId,
CASE WHEN @Lang='ar' THEN c.CategoryNameAR ELSE c.CategoryNameEN END AS CategoryName
FROM Categories c 
DECLARE @FirstCategoryId INT = (
SELECT TOP 1 c.CategoryId
FROM Categories c
ORDER BY c.CategoryID
);
SELECT
b.BookId,
lang_title.Value AS Title,
lang_author.Value AS AuthorName,
b.CoverImage as CoverImageUrl,
CASE WHEN b.CategoryID=1THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsFirstCategory ,
CASE WHEN new_books.BookID IS NOT NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsNewBook ,
CASE WHEN recent_books.BookID IS NOT NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsMostPopular
FROM vw_Books b
CROSS APPLY dbo.fn_SelectByLanguage(@Lang, b.TitleEN, b.TitleAR) lang_title
CROSS APPLY dbo.fn_SelectByLanguage(@Lang, b.AuthorNameEN, b.AuthorNameAR) lang_author
LEFT JOIN fn_NewestBooks(@NewBooksDaysThreshold,1,10) new_books ON b.BookID = new_books.BookID
LEFT JOIN fn_MostRecentBooks(@PopularityDaysThreshold,@PopularBooksCount) recent_books ON b.BookID = recent_books.BookID
LEFT JOIN fn_FirstCategoryBooks(1,10) first_category_books ON b.BookID = first_category_books.BookID
Where b.IsActive=1 
ORDER BY b.BookID
END";
    }

}