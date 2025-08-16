using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateReservationOperationsTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Create the trigger for Cerate Reservation Record
            migrationBuilder.Sql(@"
CREATE TRIGGER TR_ReservationRecord_Insert_Audit
ON ReservationRecords
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT = ISNULL(CAST(SESSION_CONTEXT(N'UserId') AS INT), NULL);
    
    INSERT INTO ReservationAudits 
    (
        ReservationID,
        Action,
        BorrowingID,
        UserID,
        Timestamp
    )
    SELECT
        i.ReservationRecordID,                    -- ReservationID
        1,										  -- ReservationCreated = 1
        NULL,									  -- BorrowingID (NULL للإنشاء الجديد)
        @UserId,								  -- UserID من SESSION_CONTEXT
        GETDATE()								  -- Timestamp
    FROM inserted i;
END;");

            //Create the trigger for change Reservation Record status 
            migrationBuilder.Sql(@"
CREATE TRIGGER TR_ReservationRecord_Update_Audit
ON ReservationRecords
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- ===== خروج مبكر إذا لم تتغير الحالة =====
    IF NOT EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN deleted d ON i.ReservationRecordID = d.ReservationRecordID
        WHERE i.Status <> d.Status
    )
    BEGIN
        RETURN;
    END;

    DECLARE @UserId INT = ISNULL(CAST(SESSION_CONTEXT(N'UserId') AS INT), NULL);

    -- ===== تسجيل جميع التغييرات (عدا Fulfilled) =====
    INSERT INTO ReservationAudits
    (
        ReservationID,
        Action,
        BorrowingID,
        UserID,
        Timestamp
    )
    SELECT
        i.ReservationRecordID,
        CASE i.Status
            WHEN 2 THEN 2 -- ConvertedToNotified
            WHEN 4 THEN 4 -- Expired
            WHEN 5 THEN 5 -- Canceled
        END AS Action,
        NULL, 
        @UserId,
        GETDATE()
    FROM inserted i
    INNER JOIN deleted d ON i.ReservationRecordID = d.ReservationRecordID
    WHERE i.Status <> d.Status
      AND i.Status <> 1     -- ignore Pending status
      AND i.Status <> 3     -- ignore Fulfilled status
      AND (
          (i.Status = 2 AND d.Status = 1) OR -- Pending → Notified
          (i.Status = 4 AND d.Status = 2) OR -- Notified → Expired
          (i.Status = 5 AND d.Status = 1)    -- Pending → Cancelled
      );
END;");

        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
