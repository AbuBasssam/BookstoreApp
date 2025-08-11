using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CompleteNewDatabaseVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "ReservationRecords");

            migrationBuilder.DropColumn(
                name: "ActualBorrowingDate",
                table: "BorrowingRecords");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "BorrowingRecords");

            migrationBuilder.RenameColumn(
                name: "InitialBorrowingDate",
                table: "BorrowingRecords",
                newName: "BorrowingDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "ReservationRecords",
                type: "datetime2(7)",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "ReservationRecords",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)1);

            migrationBuilder.AddColumn<byte>(
                name: "Type",
                table: "ReservationRecords",
                type: "tinyint",
                nullable: false);


            migrationBuilder.CreateTable(
                name: "BorrowingAudits",
                columns: table => new
                {
                    AuditID = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    BorrowingID = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<byte>(type: "tinyint", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: false),
                    OldDueDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    NewDueDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorrowingAudits", x => x.AuditID);
                    table.ForeignKey(
                        name: "FK_BorrowingAudits_BorrowingRecords_BorrowingID",
                        column: x => x.BorrowingID,
                        principalTable: "BorrowingRecords",
                        principalColumn: "BorrowingRecordID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BorrowingAudits_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });//BorrowingAudits

            migrationBuilder.CreateTable(
                name: "ReservationAudits",
                columns: table => new
                {
                    AuditID = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    ReservationID = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<byte>(type: "tinyint", nullable: false),
                    BorrowingID = table.Column<int>(type: "int", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationAudits", x => x.AuditID);
                    table.ForeignKey(
                        name: "FK_ReservationAudits_BorrowingRecords_BorrowingID",
                        column: x => x.BorrowingID,
                        principalTable: "BorrowingRecords",
                        principalColumn: "BorrowingRecordID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationAudits_ReservationRecords_ReservationID",
                        column: x => x.ReservationID,
                        principalTable: "ReservationRecords",
                        principalColumn: "ReservationRecordID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationAudits_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });//ReservationAudits

            migrationBuilder.CreateIndex(
                name: "IX_BorrowingAudits_BorrowingID",
                table: "BorrowingAudits",
                column: "BorrowingID");//BorrowingAudits index By BorrowingID

            migrationBuilder.CreateIndex(
                name: "IX_ReservationAudits_ReservationID",
                table: "ReservationAudits",
                column: "ReservationID");//ReservationAudits index By ReservationID


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(name: "BorrowingAudits");

            migrationBuilder.DropTable(name: "ReservationAudits");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "ReservationRecords");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ReservationRecords");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ReservationRecords");

            migrationBuilder.RenameColumn(
                name: "BorrowingDate",
                table: "BorrowingRecords",
                newName: "InitialBorrowingDate");

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "ReservationRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "AdminID",
                table: "BorrowingRecords",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualBorrowingDate",
                table: "BorrowingRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "BorrowingRecords",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)1);
        }
    }
}
