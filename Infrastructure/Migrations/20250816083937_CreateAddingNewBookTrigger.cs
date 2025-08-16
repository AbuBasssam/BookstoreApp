using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateAddingNewBookTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create the trigger for Add New Book Record
            migrationBuilder.Sql(@"
CREATE TRIGGER TR_Book_Insert_Audit
ON ReservationRecords
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

  
    DECLARE @UserId INT = CAST(SESSION_CONTEXT(N'UserId') AS INT);

    INSERT INTO BookAuditLogs
    (
        BookID,
        CopyID,
        UpdatedFieldName,
        OldValue,
        NewValue,
        ActionType,
        ActionDate,
        ByUserID
    )
    SELECT
        i.BookID,
        NULL,                   
        NULL,                   
        NULL,                   
        NULL,                   
        6,                      
        GETDATE(),
        @UserId
    FROM inserted i;
END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
