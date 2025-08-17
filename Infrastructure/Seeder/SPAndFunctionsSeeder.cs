using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder;
public class SPAndFunctionsSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {

        var connection = context.Database.GetDbConnection();

        await connection.OpenAsync();

        await ExecuteSqlAsync(connection, _GetBookReservableFunction());
        await ExecuteSqlAsync(connection, _GetBookBorrowableFunction());
        await ExecuteSqlAsync(connection, _GetBookRatingFunction());
    }
    private static async Task ExecuteSqlAsync(System.Data.Common.DbConnection connection, string sql)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        await command.ExecuteNonQueryAsync();
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
}
