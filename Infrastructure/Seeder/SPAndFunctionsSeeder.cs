namespace Infrastructure.Seeder;
public class SPAndFunctionsSeeder
{
    /*
     CREATE FUNCTION dbo.fn_GetBookCopyCount(@BookID int)
RETURNS int
AS
BEGIN
    DECLARE @count int;
    SELECT @count = COUNT(*) FROM BookCopies WHERE BookID = @BookID;
    RETURN ISNULL(@count, 0);
END
     */
}
