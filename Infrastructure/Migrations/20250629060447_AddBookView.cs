using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBookView : Migration
    {

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE OR ALTER VIEW dbo.BookView AS
            SELECT 
                b.BookID ,
                b.TitleEN,
                b.TitleAR,
                b.PublishDate,
                b.ISBN,
                b.CategoryID,
                b.AuthorID,
                b.ImageUrl,
                CASE WHEN (
                    SELECT COUNT(*) 
                    FROM BookCopies bc 
                    WHERE bc.BookID = b.BookID AND bc.IsAvailable = 1
                ) > 0 THEN CAST(1 AS BIT)
                ELSE CAST(0 AS BIT)
                END AS IsAvailable
            FROM 
                Books b;
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS dbo.BookView");
        }
    }
}
