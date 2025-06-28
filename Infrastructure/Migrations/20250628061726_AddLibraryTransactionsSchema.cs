using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLibraryTransactionsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BorrowingRecords",
                columns: table => new
                {
                    BorrowingRecordID = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    BookCopyID = table.Column<int>(type: "int", nullable: false),
                    MemberID = table.Column<int>(type: "int", nullable: false),
                    InitialBorrowingDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ActualBorrowingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RenewalCount = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    Status = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    AdminID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorrowingRecords", x => x.BorrowingRecordID);
                    table.ForeignKey(
                        name: "FK_BorrowingRecords_BookCopies_BookCopyID",
                        column: x => x.BookCopyID,
                        principalTable: "BookCopies",
                        principalColumn: "CopyID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BorrowingRecords_Users_AdminID",
                        column: x => x.AdminID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BorrowingRecords_Users_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });//BorrowingRecords

            migrationBuilder.CreateTable(
                name: "ReservationRecords",
                columns: table => new
                {
                    ReservationRecordID = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    BookID = table.Column<int>(type: "int", nullable: false),
                    MemberID = table.Column<int>(type: "int", nullable: false),
                    ReservationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationRecords", x => x.ReservationRecordID);
                    table.ForeignKey(
                        name: "FK_ReservationRecords_Books_BookID",
                        column: x => x.BookID,
                        principalTable: "Books",
                        principalColumn: "BookID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationRecords_Users_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });//ReservationRecords

            migrationBuilder.CreateTable(
                name: "Fines",
                columns: table => new
                {
                    FineID = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    BorrowingID = table.Column<int>(type: "int", nullable: false),
                    TotalLateDays = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    Amount = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fines", x => x.FineID);
                    table.ForeignKey(
                        name: "FK_Fines_BorrowingRecords_BorrowingID",
                        column: x => x.BorrowingID,
                        principalTable: "BorrowingRecords",
                        principalColumn: "BorrowingRecordID",
                        onDelete: ReferentialAction.Restrict);
                });// Fines

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    SettingID = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    MaxLoanDays = table.Column<int>(type: "int", nullable: false, defaultValue: 14),
                    MaxRenewals = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    RenewalExtensionDays = table.Column<int>(type: "int", nullable: false, defaultValue: 7),
                    FinePerDay = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MaxLoansPerMember = table.Column<int>(type: "int", nullable: false),
                    ReservationExpiryDays = table.Column<int>(type: "int", nullable: false),
                    GracePeriodDays = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.SettingID);
                });// SystemSettings

            migrationBuilder.CreateIndex(
                name: "IX_BorrowingRecords_MemberID",
                table: "BorrowingRecords",
                column: "MemberID");//BorrowingRecords by MemberID

            migrationBuilder.CreateIndex(
                name: "IX_Fines_BorrowingID",
                table: "Fines",
                column: "BorrowingID");// Fines by BorrowingID

            migrationBuilder.CreateIndex(
                name: "IX_ReservationRecords_MemberID",
                table: "ReservationRecords",
                column: "MemberID");// ReservationRecords by MemberID

            /*migrationBuilder.CreateIndex(
                name: "IX_ReservationRecords_BookID",
                table: "ReservationRecords",
                column: "BookID");*/


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Fines");

            migrationBuilder.DropTable(name: "ReservationRecords");

            migrationBuilder.DropTable(name: "SystemSettings");

            migrationBuilder.DropTable(name: "BorrowingRecords");
        }
    }
}
