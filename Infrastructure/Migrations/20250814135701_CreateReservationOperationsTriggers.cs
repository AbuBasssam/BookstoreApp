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
            migrationBuilder.Sql(@"CREATE TRIGGER TR_ReservationRecord_Insert_Audit
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
            migrationBuilder.Sql(@"CREATE TRIGGER TR_ReservationRecord_Update_Audit
ON ReservationRecords
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT = ISNULL(CAST(SESSION_CONTEXT(N'UserId') AS INT), NULL);
    
    -- تسجيل تحويل الحجز إلى ""تم الإشعار"" (Pending → Notified)
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
        3,                       -- ConvertedToNotified = 3
        NULL,                    -- BorrowingID (NULL للإشعار)
        @UserId,                 -- UserID من SESSION_CONTEXT
        GETDATE()                -- Timestamp
    FROM inserted i
    INNER JOIN deleted d ON i.ReservationRecordID = d.ReservationRecordID
    WHERE i.Status = 2           -- Notified = 2
      AND d.Status = 1;          -- كان Pending = 1

    -- تسجيل تحويل الحجز إلى ""مكتمل"" (Notified → Fulfilled)
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
        2,                       -- ConvertedToFulfilled = 2
        -- يمكن ربط BorrowingID هنا إذا كان متوفراً في الاستعلام
        (SELECT TOP 1 br.BorrowingRecordID 
         FROM BorrowingRecords br 
         WHERE br.MemberID = i.MemberID 
           AND br.BookCopyID IN (
               SELECT bc.CopyID 
               FROM BookCopies bc 
               WHERE bc.BookID = i.BookID
           )
           AND br.BorrowDate >= i.ReservationDate
         ORDER BY br.BorrowDate DESC),
        @UserId,                 -- UserID من SESSION_CONTEXT
        GETDATE()                -- Timestamp
    FROM inserted i
    INNER JOIN deleted d ON i.ReservationRecordID = d.ReservationRecordID
    WHERE i.Status = 3           -- Fulfilled = 3
      AND d.Status IN (1, 2);    -- كان Pending أو Notified

    -- تسجيل تحويل الحجز إلى ""منتهي الصلاحية"" (Any Status → Expired)
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
        4,                       -- Expired = 4
        NULL,                    -- BorrowingID (NULL للانتهاء)
        @UserId,                 -- UserID من SESSION_CONTEXT
        GETDATE()                -- Timestamp
    FROM inserted i
    INNER JOIN deleted d ON i.ReservationRecordID = d.ReservationRecordID
    WHERE i.Status = 4           -- Expired = 4
      AND d.Status <> 4          -- لم يكن منتهي الصلاحية من قبل

    -- تسجيل إلغاء الحجز (Any Status → Cancelled)
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
        5,                       -- Canceled = 5
        NULL,                    -- BorrowingID (NULL للإلغاء)
        @UserId,                 -- UserID من SESSION_CONTEXT
        GETDATE()                -- Timestamp
    FROM inserted i
    INNER JOIN deleted d ON i.ReservationRecordID = d.ReservationRecordID
    WHERE i.Status = 5           -- Cancelled = 5
      AND d.Status <> 5          -- لم يكن ملغياً من قبل
    
END;");

        }
        //        CREATE TRIGGER TR_ReservationRecord_Update_Audit
        //ON ReservationRecords
        //AFTER UPDATE
        //AS
        //BEGIN
        //    SET NOCOUNT ON;

        //            -- ===== تحسين الأداء: فحص سريع للخروج المبكر =====
        //            --إذا لم يتغير حقل Status في أي سجل، اخرج من التريجر فوراً
        //    IF NOT EXISTS(
        //        SELECT 1
        //        FROM inserted i
        //        INNER JOIN deleted d ON i.ReservationRecordID = d.ReservationRecordID
        //        WHERE i.Status<> d.Status
        //    )
        //    BEGIN
        //        RETURN; --خروج مبكر - لا توجد تغييرات في الحالة
        //    END

        //    DECLARE @UserId INT = ISNULL(CAST(SESSION_CONTEXT(N'UserId') AS INT), NULL);

        //            -- ===== تسجيل جميع تغييرات الحالة في استعلام واحد محسن =====
        //            INSERT INTO ReservationAudits
        //            (
        //                ReservationID,
        //                Action,
        //                BorrowingID,
        //                UserID,
        //                Timestamp


        //            )
        //    SELECT
        //        i.ReservationRecordID,
        //        --تحديد نوع الإجراء بناءً على الحالة الجديدة
        //        CASE i.Status
        //            WHEN 2 THEN 2-- ConvertedToNotified
        //            WHEN 3 THEN 3-- ConvertedToFulfilled
        //            WHEN 4 THEN 4-- Expired
        //            WHEN 5 THEN 5-- Canceled
        //        END AS Action,
        //        --ربط BorrowingID فقط للحالة Fulfilled


        //        CASE
        //            WHEN i.Status = 3 THEN-- Fulfilled
        //                (SELECT TOP 1 br.BorrowingRecordID
        //                 FROM BorrowingRecords br
        //                 INNER JOIN BookCopies bc ON br.BookCopyID = bc.CopyID
        //                 WHERE br.MemberID = i.MemberID
        //                   AND bc.BookID = i.BookID
        //                   AND br.BorrowingDate >= i.ReservationDate
        //                   AND br.BorrowingDate <= GETDATE()

        //                   AND(d.Status<> 4 OR d.Status<>5)
        //                 ORDER BY br.BorrowingDate DESC)
        //            ELSE NULL
        //        END AS BorrowingID,
        //        @UserId,
        //        GETDATE()
        //    FROM inserted i
        //    INNER JOIN deleted d ON i.ReservationRecordID = d.ReservationRecordID
        //    WHERE i.Status<> d.Status-- فقط السجلات التي تغيرت حالتها
        //      AND i.Status<>1-- الحالات المطلوب تسجيلها
        //      AND(
        //          --شروط انتقال الحالة المنطقية
        //          (i.Status = 2 AND d.Status = 1) OR-- Pending → Notified
        //          (i.Status = 3 AND d.Status = 2) OR-- Notified → Fulfilled
        //          (i.Status = 4 AND d.Status = 2) OR-- Notified → Expired
        //          (i.Status = 5 AND d.Status = 1)-- Pending → Cancelled
        //      );
        //            END;


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
