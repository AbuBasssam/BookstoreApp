using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBookTrackingAndUserRatings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookAuditLogs",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    BookID = table.Column<int>(type: "int", nullable: false),
                    CopyID = table.Column<int>(type: "int", nullable: true),
                    UpdatedFieldName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ActionType = table.Column<byte>(type: "tinyint", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookAuditLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BookAuditLogs_BookCopies_CopyID",
                        column: x => x.CopyID,
                        principalTable: "BookCopies",
                        principalColumn: "CopyID");
                    table.ForeignKey(
                        name: "FK_BookAuditLogs_Books_BookID",
                        column: x => x.BookID,
                        principalTable: "Books",
                        principalColumn: "BookID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookAuditLogs_Users_ByUserID",
                        column: x => x.ByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });//BookAuditLogs

            migrationBuilder.CreateTable(
                name: "BookRatings",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    BookID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookRatings", x => x.ID);
                    table.CheckConstraint("CK_BookRating_Rating", "[Rating] >= 1 AND [Rating] <= 5");
                    table.ForeignKey(
                        name: "FK_BookRatings_Books_BookID",
                        column: x => x.BookID,
                        principalTable: "Books",
                        principalColumn: "BookID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookRatings_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });//BookRatings

            migrationBuilder.CreateIndex(
                name: "IX_BookAuditLog_ByUserID",
                table: "BookAuditLogs",
                column: "ByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_BookAuditLogs_BookID",
                table: "BookAuditLogs",
                column: "BookID");



            migrationBuilder.CreateIndex(
                name: "IX_BookRating_BookID",
                table: "BookRatings",
                column: "BookID");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookAuditLogs");

            migrationBuilder.DropTable(
                name: "BookRatings");
        }
    }
}
