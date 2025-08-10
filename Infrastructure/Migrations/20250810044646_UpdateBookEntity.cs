using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Books",
                newName: "CoverImage");

            migrationBuilder.AddColumn<DateOnly>(
                name: "AvailabilityDate",
                table: "Books",
                type: "date",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAR",
                table: "Books",
                type: "nvarchar(300)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEN",
                table: "Books",
                type: "nvarchar(300)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LanguageID",
                table: "Books",
                type: "int",
                nullable: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastReservationOpenDate",
                table: "Books",
                type: "datetime2(7)",
                nullable: false);

            migrationBuilder.AddColumn<short>(
                name: "PageCount",
                table: "Books",
                type: "smallint",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "Books",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                comment: "Format: [A-Z][2 digits] or [A-Z][2 digits]-[alphanumeric]");

            migrationBuilder.AddColumn<int>(
                name: "PublisherID",
                table: "Books",
                type: "int",
                nullable: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnHold",
                table: "BookCopies",
                type: "bit",
                nullable: false,
                defaultValue: false);


            migrationBuilder.AddCheckConstraint(
                name: "CK_Books_Position_Format",
                table: "Books",
                sql: "Position LIKE '[A-Z][0-9][0-9]%' OR Position LIKE '[A-Z][0-9][0-9]-[0-9A-Za-z]%'");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Languages_LanguageID",
                table: "Books",
                column: "LanguageID",
                principalTable: "Languages",
                principalColumn: "LanguageID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Publishers_PublisherID",
                table: "Books",
                column: "PublisherID",
                principalTable: "Publishers",
                principalColumn: "PublisherID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Languages_LanguageID",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Publishers_PublisherID",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_LanguageID",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_PublisherID",
                table: "Books");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Books_Position_Format",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "AvailabilityDate",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "DescriptionAR",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "DescriptionEN",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "LanguageID",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "LastReservationOpenDate",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "PageCount",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "PublisherID",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "IsOnHold",
                table: "BookCopies");

            migrationBuilder.RenameColumn(
                name: "CoverImage",
                table: "Books",
                newName: "ImageUrl");
        }
    }
}
