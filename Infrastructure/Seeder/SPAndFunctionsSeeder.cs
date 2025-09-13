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
        await SeederHelper.ExecuteSqlAsync(connection, _SP_GetPagedBooksByCategory());
        await SeederHelper.ExecuteSqlAsync(connection, _Sp_GetPagedNewestBooks());
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
CREATE OR ALTER FUNCTION dbo.fn_NewestBooks( @NewBooksDateThreshold DATE, @NewBooksDaysThreshold INT)
RETURNS TABLE
AS
RETURN
(
    SELECT b.BookID,b.AvailabilityDate
    FROM Books b
    WHERE b.IsActive = 1 AND
	b.AvailabilityDate >= DATEADD(DAY, -@NewBooksDaysThreshold, @NewBooksDateThreshold)
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
CREATE OR ALTER PROCEDURE SP_GetHomePageData
@NewBooksDaysThreshold INT,
@PopularityDaysThreshold INT,
@PopularBooksCount INT,
@FirstCategoryPageSize INT=10,
@NewBooksPageSize INT=10
AS
BEGIN
SET NOCOUNT ON;
DECLARE @PageNumber INT=1

SELECT
c.CategoryId,
c.CategoryNameEN,
c.CategoryNameAR
FROM Categories c 

DECLARE @FirstCategoryId INT = (
SELECT TOP 1 c.CategoryId
FROM Categories c
ORDER BY c.CategoryID
);

DECLARE @NewestPage TABLE(BookID INT PRIMARY KEY);

INSERT INTO @NewestPage (BookID)
SELECT BookID
FROM fn_NewestBooks(GETDATE(), @NewBooksDaysThreshold)NB
Order By NB.AvailabilityDate DESC
OFFSET (@PageNumber - 1) * @NewBooksPageSize ROWS
FETCH NEXT @NewBooksPageSize ROWS ONLY;

SELECT
b.BookId,
b.TitleEN,
b.TitleAR,
b.AuthorNameEN,
b.AuthorNameAR,
b.CoverImage as CoverImageUrl,
CASE WHEN b.CategoryID=1THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsFirstCategory ,
CASE WHEN new_books.BookID IS NOT NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsNewBook ,
CASE WHEN recent_books.BookID IS NOT NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsMostPopular
FROM vw_Books b
LEFT JOIN @NewestPage  new_books ON b.BookID = new_books.BookID
LEFT JOIN fn_MostRecentBooks(@PopularityDaysThreshold,@PopularBooksCount) recent_books ON b.BookID = recent_books.BookID
LEFT JOIN fn_FirstCategoryBooks(@PageNumber,@FirstCategoryPageSize) first_category_books ON b.BookID = first_category_books.BookID
Where b.IsActive=1
AND (
    b.CategoryID = 1 OR -- First Category
    new_books.BookID IS NOT NULL OR -- New Books  
    recent_books.BookID IS NOT NULL -- Popular Books
)
ORDER BY b.BookID;
WITH fc_BookCounts AS
(
    SELECT COUNT(BookID) AS TotalFirstCategoryBooks
    FROM Books
    WHERE CategoryID = @FirstCategoryId
)
,
NewBooks AS
(
    SELECT COUNT(BookID) AS TotalNewBooks
    FROM 
	fn_NewestBooks(GETDATE(),@NewBooksDaysThreshold)
	
)
SELECT 
    f.TotalFirstCategoryBooks,
    n.TotalNewBooks,
    @FirstCategoryPageSize AS FirstCategoryPageSize,
    @NewBooksPageSize AS NewBooksPageSize
FROM fc_BookCounts f
CROSS JOIN NewBooks n

END";
    }
    private static string _SP_GetPagedBooksByCategory()
    {
        return @"
CREATE OR ALTER PROCEDURE SP_GetPagedBooksByCategory
    @CategoryID INT,
	@PageNumber INT,
	@PageSize INT,
	@LangCode CHAR(2),
	@NewBookDateThreshold DATE
AS
BEGIN
	
	SET NOCOUNT ON;
	DECLARE @Newest TABLE(BookID INT PRIMARY KEY);

	INSERT INTO @Newest (BookID)
	SELECT BookID
	FROM fn_NewestBooks(GETDATE(), 30)new_books
	Order By new_books.AvailabilityDate DESC
	
		Select
		b.BookId,
		Title.Value AS Title,
		Author.Value AS Author,
		b.CoverImage as CoverImageUrl,
		CASE WHEN new_books.BookID IS NOT NULL
			THEN CAST(1 AS BIT)
			ELSE CAST(0 AS BIT) END AS IsNewBook 
	
	    FROM vw_Books b
		LEFT JOIN @Newest new_books  ON b.BookID = new_books.BookID
		CROSS APPLY dbo.fn_SelectByLanguage(@LangCode, b.TitleEN, b.TitleAR) Title
		CROSS APPLY dbo.fn_SelectByLanguage(@LangCode, b.AuthorNameEN, b.AuthorNameAR) Author
	
		Where b.IsActive=1 AND b.CategoryID=@CategoryID
		Order By b.BookID
	
		OFFSET (@PageNumber - 1) * @PageSize ROWS
	    FETCH NEXT @PageSize ROWS ONLY;
		
	With TotalCount as
	(
		Select Count(BookId) as BookCount from vw_Books
		where CategoryID=@CategoryID 
	)
	Select 
		BookCount as TotalCount,
		CEILING(1.0 * BookCount / @PageSize) AS TotalPages
	From TotalCount
	
	END";
    }
    private static string _Sp_GetPagedNewestBooks()
    {
        return @"
CREATE OR ALTER PROCEDURE SP_GetPagedNewestBooks
     @PageNumber INT,
    @PageSize INT,
	@LangCode CHAR(2)='en',
    @NewBooksDateThreshold DATETIME = NULL,
    @NewBooksDaysThreshold INT = 30

AS
BEGIN
    SET NOCOUNT ON;

    SET @NewBooksDateThreshold = ISNULL(@NewBooksDateThreshold, GETDATE());
	 DECLARE @Newest TABLE
    (
        BookID INT PRIMARY KEY
    );
	INSERT INTO @Newest (BookID)
    SELECT nb.BookID
    FROM dbo.fn_NewestBooks(@NewBooksDateThreshold, @NewBooksDaysThreshold) nb;

    SELECT 
        b.BookId,
        Title.Value AS Title,
        Author.Value AS Author,
        b.CoverImage AS CoverImageUrl,
		CASE WHEN nb.BookID IS NOT NULL
			THEN CAST(1 AS BIT)
			ELSE CAST(0 AS BIT) END AS IsNewBook 
    FROM dbo.vw_Books b 
    Left JOIN @Newest nb  ON nb.BookID = b.BookID
    CROSS APPLY dbo.fn_SelectByLanguage(@LangCode, b.TitleEN, b.TitleAR) Title
    CROSS APPLY dbo.fn_SelectByLanguage(@LangCode, b.AuthorNameEN, b.AuthorNameAR) Author
    ORDER BY b.AvailabilityDate DESC
	OFFSET (@PageNumber - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY;

	 SELECT 
        COUNT(BookId) AS TotalCount, 
        CEILING(1.0 * COUNT(BookId) / @PageSize) AS TotalPages
    FROM @Newest;
END;";
    }

}