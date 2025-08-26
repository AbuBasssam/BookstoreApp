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
SELECT TOP @MostPopularBooksCount  bc.BookID 
                FROM BorrowingRecords br INNER join BookCopies bc 
				on br.BookCopyID=bc.CopyID
				WHERE br.BorrowingDate >= DATEADD(DAY, -@PopularityDaysThreshold, GETDATE())
                GROUP BY bc.BookID 
                ORDER BY COUNT(*) DESC
    );";
    }

}