using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGetBookCopyCountFunction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS dbo.fn_GetBookCopyCount;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE FUNCTION dbo.fn_GetBookCopyCount(@BookID int)
            RETURNS int
            AS
            BEGIN
                DECLARE @count int;
                SELECT @count = COUNT(*) FROM BookCopies WHERE BookID = @BookID;
                RETURN ISNULL(@count, 0);
            END
            ");

        }
    }
}
