using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CompleteNotificationModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_UserDevices_UserDeviceId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_UserId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "RelatedEntityId",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "UserDeviceId",
                table: "Notifications",
                newName: "UserDeviceID");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Notifications",
                newName: "TitleEN");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "Notifications",
                newName: "MessageEN");


            migrationBuilder.AddColumn<string>(
                name: "MessageAR",
                table: "Notifications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TitleAR",
                table: "Notifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");



            migrationBuilder.CreateTable(
                name: "BorrowNotifications",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    NotificationID = table.Column<int>(type: "int", nullable: false),
                    BorrowID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorrowNotification", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BorrowNotification_BorrowingRecords_BorrowID",
                        column: x => x.BorrowID,
                        principalTable: "BorrowingRecords",
                        principalColumn: "BorrowingRecordID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BorrowNotification_Notifications_NotificationID",
                        column: x => x.NotificationID,
                        principalTable: "Notifications",
                        principalColumn: "NotificationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReservationNotifications",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    NotificationID = table.Column<int>(type: "int", nullable: false),
                    ReservationID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationNotification", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReservationNotification_Notifications_NotificationID",
                        column: x => x.NotificationID,
                        principalTable: "Notifications",
                        principalColumn: "NotificationID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservationNotification_ReservationRecords_ReservationID",
                        column: x => x.ReservationID,
                        principalTable: "ReservationRecords",
                        principalColumn: "ReservationRecordID",
                        onDelete: ReferentialAction.Cascade);
                });







            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_UserDevices_UserDeviceID",
                table: "Notifications",
                column: "UserDeviceID",
                principalTable: "UserDevices",
                principalColumn: "DeviceID",
                onDelete: ReferentialAction.Restrict);


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookAuditLogs_Books_BookID",
                table: "BookAuditLogs");


            migrationBuilder.DropForeignKey(
                name: "FK_BookRatings_Books_BookID",
                table: "BookRatings");



            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_UserDevices_UserDeviceID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_UserId",
                table: "Notifications");

            migrationBuilder.DropTable(name: "BorrowNotification");

            migrationBuilder.DropTable(name: "ReservationNotification");





            migrationBuilder.DropColumn(
                name: "MessageAR",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TitleAR",
                table: "Notifications");



            migrationBuilder.AddForeignKey(
                name: "FK_BookAuditLogs_Books_BookID",
                table: "BookAuditLogs",
                column: "BookID",
                principalTable: "Books",
                principalColumn: "BookID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookRatings_Books_BookID",
                table: "BookRatings",
                column: "BookID",
                principalTable: "Books",
                principalColumn: "BookID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_UserDevices_UserDeviceId",
                table: "Notifications",
                column: "UserDeviceId",
                principalTable: "UserDevices",
                principalColumn: "DeviceID",
                onDelete: ReferentialAction.Restrict);


        }
    }
}
