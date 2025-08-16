using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateBorrowingsOperationTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Create the trigger for Cerate Borrowing Record
            migrationBuilder.Sql(@"CREATE TRIGGER TR_BorrowingRecord_Insert_Audit
ON BorrowingRecords
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT = ISNULL(CAST(SESSION_CONTEXT(N'UserId') AS INT), NULL);
    
    INSERT INTO BorrowingAudits 
    (
        BorrowingID,
        Action,
        UserID,
        Timestamp,
        OldDueDate,
        NewDueDate
    )
    SELECT
        i.BorrowingRecordID,                    -- BorrowingID
        1,                       -- BorrowCreated = 1
        @UserId,                 -- UserID من SESSION_CONTEXT
        GETDATE(),               -- Timestamp
        NULL,                    -- OldDueDate (NULL للإنشاء الجديد)
        NULL                     -- NewDueDate
    FROM inserted i;
	

END;");

            // Create the trigger for update Borrowing Record
            migrationBuilder.Sql(@"CREATE TRIGGER TR_BorrowingRecord_Update_Audit
            ON BorrowingRecords
			AFTER UPDATE
			As
Begin
SET NOCOUNT ON;
    DECLARE @UserId INT = ISNULL(CAST(SESSION_CONTEXT(N'UserId') AS INT), NULL);
	INSERT INTO BorrowingAudits 
    (
        BorrowingID,
        Action,
        UserID,
        Timestamp,
        OldDueDate,
        NewDueDate
    )
    SELECT
        i.BorrowingRecordID,                    -- BorrowingID
        2,                       -- BorrowExtended = 2
        @UserId,                 -- UserID من SESSION_CONTEXT
        GETDATE(),               -- Timestamp
        d.DueDate,               -- OldDueDate
        i.DueDate                -- NewDueDate
    FROM inserted i
    INNER JOIN deleted d ON i.BorrowingRecordID = d.BorrowingRecordID
    WHERE i.DueDate <> d.DueDate 
	INSERT INTO BorrowingAudits 
    (
        BorrowingID,
        Action,
        UserID,
        Timestamp,
        OldDueDate,
        NewDueDate
    )
    SELECT
        i.BorrowingRecordID,                    -- BorrowingID
        3,                                      -- BorrowReturned = 3
        @UserId,                                -- UserID من SESSION_CONTEXT
        GETDATE(),                              -- Timestamp
        NULL,                                   -- OldDueDate
        NULL                                    -- NewDueDate (NULL للإرجاع)
    FROM inserted i
    INNER JOIN deleted d ON i.BorrowingRecordID = d.BorrowingRecordID
    WHERE 
        (i.ReturnDate IS NOT NULL AND d.ReturnDate IS NULL) -- تم إضافة تاريخ الإرجاع
    
End;");



        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
