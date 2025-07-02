using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdjustOtpTypeConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Otp_Type",
                table: "Otps");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Otp_Type",
                table: "Otps",
                sql: "Type > 0 AND Type < 3");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Otp_Type",
                table: "Otps");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Otp_Type",
                table: "Otps",
                sql: "Type > 0 AND Type < 2");
        }
    }
}
