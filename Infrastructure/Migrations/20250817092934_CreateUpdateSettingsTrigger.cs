using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateUpdateSettingsTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR ALTEr TRIGGER TR_SystemSettings_Update_Audit
ON dbo.SystemSettings
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserId INT = ISNULL(CAST(SESSION_CONTEXT(N'UserId') AS INT), 0);

    INSERT INTO dbo.SystemSettingsAudits
        (SettingName, OldValue, NewValue, ChangedBy, ChangeDate)
    SELECT 
        v.SettingName,
        v.OldValue,
        v.NewValue,
        @UserId,
        GETDATE()
    FROM inserted i
    INNER JOIN deleted d ON 1 = 1
    CROSS APPLY
    (
        VALUES
            ('MaxLoanDays',         CAST(d.MaxLoanDays AS NVARCHAR(50)),         CAST(i.MaxLoanDays AS NVARCHAR(50))),
            ('MaxRenewals',         CAST(d.MaxRenewals AS NVARCHAR(50)),         CAST(i.MaxRenewals AS NVARCHAR(50))),
            ('RenewalExtensionDays',CAST(d.RenewalExtensionDays AS NVARCHAR(50)),CAST(i.RenewalExtensionDays AS NVARCHAR(50))),
            ('FinePerDay',          CAST(d.FinePerDay AS NVARCHAR(50)),          CAST(i.FinePerDay AS NVARCHAR(50))),
            ('MaxLoansPerMember',   CAST(d.MaxLoansPerMember AS NVARCHAR(50)),   CAST(i.MaxLoansPerMember AS NVARCHAR(50))),
            ('ReservationExpiryDays',CAST(d.ReservationExpiryDays AS NVARCHAR(50)),CAST(i.ReservationExpiryDays AS NVARCHAR(50))),
            ('PickupExpiryHours',   CAST(d.PickupExpiryHours AS NVARCHAR(50)),   CAST(i.PickupExpiryHours AS NVARCHAR(50)))
    ) v (SettingName, OldValue, NewValue)
    WHERE v.OldValue <> v.NewValue;
END;");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"Drop Trigger TR_SystemSettings_Update_Audit");

        }
    }
}
