using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder;

public static class TriggerSeeder
{
    public static async Task SeedAsync(DbContext context)
    {
        var connection = context.Database.GetDbConnection();

        await connection.OpenAsync();

        await SeederHelper.ExecuteSqlAsync(connection, _BookInsertAuditTrigger());
        await SeederHelper.ExecuteSqlAsync(connection, _BookUpdateAuditTrigger());
        await SeederHelper.ExecuteSqlAsync(connection, _BorrowingRecordInsertAuditTrigger());
        await SeederHelper.ExecuteSqlAsync(connection, _BorrowingRecordUpdateAuditTrigger());
        await SeederHelper.ExecuteSqlAsync(connection, _BorrowingRecordInsteadOfInsertTrigger());
        await SeederHelper.ExecuteSqlAsync(connection, _ReservationRecordInsertAuditTrigger());
        await SeederHelper.ExecuteSqlAsync(connection, _ReservationRecordUpdateAuditTrigger());
        await SeederHelper.ExecuteSqlAsync(connection, _SystemSettingsUpdateAuditTrigger());
        await SeederHelper.ExecuteSqlAsync(connection, _AddBookCopyInsertAuditTrigger());
    }
    private static string _BookInsertAuditTrigger()
    {
        return @"
CREATE OR ALTER TRIGGER TR_Book_Insert_Audit
ON Books
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

  
    DECLARE @UserId INT = CAST(SESSION_CONTEXT(N'UserId') AS INT);

    INSERT INTO BookActivityLogs
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
        5,                      
        GETDATE(),
        @UserId
    FROM inserted i;
END;";
    }
    private static string _BookUpdateAuditTrigger()
    {
        return @"
CREATE OR ALTER TRIGGER TR_Book_Update_Audit
                ON Books
                AFTER UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;

    DECLARE @UserId INT = ISNULL(CAST(SESSION_CONTEXT(N'UserId') AS INT), NULL);

    -------------------------------
    -- LastWaitListOpenDate Update Only
    -------------------------------
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN deleted d ON i.BookID = d.BookID
        WHERE i.LastWaitListOpenDate <> d.LastWaitListOpenDate
    )
    BEGIN
        INSERT INTO BookActivityLogs
        (
            BookID,
            UpdatedFieldName,
            OldValue,
            NewValue,
            ActionType,
            ActionDate,
            ByUserID
        )
        SELECT
            i.BookID,
            'LastWaitListOpenDate',
            CONVERT(NVARCHAR(30), d.LastWaitListOpenDate, 126),
            CONVERT(NVARCHAR(30), i.LastWaitListOpenDate, 126),
            CASE
                WHEN (d.LastWaitListOpenDate IS NULL AND i.LastWaitListOpenDate IS NOT NULL
                      OR d.LastWaitListOpenDate IS NOT NULL AND i.LastWaitListOpenDate > d.LastWaitListOpenDate)
                    THEN 1 -- OpenWaitList
                ELSE 2 -- CloseWaitList
            END,
            GETDATE(),
            @UserId
        FROM inserted i
        INNER JOIN deleted d ON i.BookID = d.BookID;
    END

    -------------------------------
    -- Deactivate Book Audit Log
    -------------------------------
    INSERT INTO BookActivityLogs
    (
        BookID,
        UpdatedFieldName,
        OldValue,
        NewValue,
        ActionType,
        ActionDate,
        ByUserID
    )
    SELECT
        i.BookID,
        'IsActive',
        CAST(d.IsActive AS NVARCHAR(10)),
        CAST(i.IsActive AS NVARCHAR(10)),
        6, -- DeactivateBook
        GETDATE(),
        @UserId
    FROM inserted i
    INNER JOIN deleted d ON i.BookID = d.BookID
    WHERE d.IsActive = 1 AND i.IsActive = 0;

    -------------------------------
    -- Update Book Information Audit Log Using UNPIVOT
    -------------------------------

    --CTE for capturing changes
    ;WITH BookChanges AS
    (
        SELECT 
            i.BookID,
            CAST(d.ISBN as nvarchar(300)) AS OldISBN, CAST(i.ISBN AS NVARCHAR(300)) AS NewISBN,
            CAST(d.TitleEN AS  NVARCHAR(300)) AS OldTitleEN,CAST(i.TitleEN AS  NVARCHAR(300)) AS NewTitleEN,
            CAST(d.TitleAR AS NVARCHAR(300)) AS OldTitleAR, CAST(i.TitleAR AS NVARCHAR(300)) AS NewTitleAR,
            CAST(d.DescriptionEN AS NVARCHAR(300)) AS OldDescriptionEN,CAST( i.DescriptionEN AS NVARCHAR(300)) AS NewDescriptionEN,
            CAST(d.DescriptionAR AS NVARCHAR(300)) AS OldDescriptionAR, CAST(i.DescriptionAR AS NVARCHAR(300)) AS NewDescriptionAR,
            CAST(d.PublisherID AS NVARCHAR(300)) AS OldPublisherID, CAST(i.PublisherID AS NVARCHAR(300)) AS NewPublisherID,
            CAST(d.AuthorID AS NVARCHAR(300)) AS OldAuthorID, CAST(i.AuthorID AS NVARCHAR(300)) AS NewAuthorID,
            CAST(d.LanguageID AS NVARCHAR(300)) AS OldLanguageID, CAST(i.LanguageID AS NVARCHAR(300)) AS NewLanguageID,
            CAST(d.CategoryID AS NVARCHAR(300)) AS OldCategoryID, CAST(i.CategoryID AS NVARCHAR(300)) AS NewCategoryID,
            CAST(d.PageCount AS NVARCHAR(300)) AS OldPageCount, CAST(i.PageCount AS NVARCHAR(300)) AS NewPageCount,
            CONVERT(NVARCHAR(300), d.PublishDate, 126) AS OldPublishDate, CONVERT(NVARCHAR(300), i.PublishDate, 126) AS NewPublishDate,
            CONVERT(NVARCHAR(300), d.AvailabilityDate, 126) AS OldAvailabilityDate, CONVERT(NVARCHAR(300), i.AvailabilityDate, 126) AS NewAvailabilityDate,
            CAST(d.Position AS NVARCHAR(300)) AS OldPosition, CAST(i.Position AS nvarchar(300)) AS NewPosition,
            CAST(d.CoverImage AS NVARCHAR(300)) AS OldCoverImage, CAST(i.CoverImage AS NVARCHAR(300))AS NewCoverImage
        FROM inserted i
        INNER JOIN deleted d ON i.BookID = d.BookID
    )
    INSERT INTO BookActivityLogs
    (
        BookID,
        UpdatedFieldName,
        OldValue,
        NewValue,
        ActionType,
        ActionDate,
        ByUserID
    )
    SELECT
        BookID,
        FieldName,
        OldValue,
        NewValue,
        4, -- UpdateBookInfo
        GETDATE(),
        @UserId
    FROM
    BookChanges
    UNPIVOT
    (
        OldValue FOR FieldName IN
        (
            OldISBN, OldTitleEN, OldTitleAR, OldDescriptionEN, OldDescriptionAR,
            OldPublisherID, OldAuthorID, OldLanguageID, OldCategoryID,
            OldPageCount, OldPublishDate, OldAvailabilityDate, OldPosition, OldCoverImage
        )
    ) AS OldVals
    UNPIVOT
    (
        NewValue FOR FieldName2 IN
        (
            NewISBN, NewTitleEN, NewTitleAR, NewDescriptionEN, NewDescriptionAR,
            NewPublisherID, NewAuthorID, NewLanguageID, NewCategoryID,
            NewPageCount, NewPublishDate, NewAvailabilityDate, NewPosition, NewCoverImage
        )
    ) AS NewVals
    WHERE
        -- Filter out unchanged fields
        OldValue <> NewValue;

END;";
    }
    private static string _BorrowingRecordInsertAuditTrigger()
    {
        return @"
CREATE OR ALTER TRIGGER TR_BorrowingRecord_Insert_Audit
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
        1,                                      -- BorrowCreated = 1
        @UserId,                                -- UserID من SESSION_CONTEXT
        GETDATE(),                              -- Timestamp
        NULL,                                   -- OldDueDate (NULL للإنشاء الجديد)
        NULL                                    -- NewDueDate
    FROM inserted i;
	

END;";
    }
    private static string _BorrowingRecordUpdateAuditTrigger()
    {
        return @"
CREATE OR ALTER TRIGGER TR_BorrowingRecord_Update_Audit
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
    
End;";
    }
    private static string _BorrowingRecordInsteadOfInsertTrigger()
    {
        return @"
CREATE OR ALTER TRIGGER TR_BorrowingRecord_Insert
ON BorrowingRecords
INSTEAD OF INSERT
AS
BEGIN
   SET NOCOUNT ON;
BEGIN TRY
BEGIN TRANSACTION;
-- 1.Insert into BorrowingRecords table 
INSERT INTO BorrowingRecords ( BookCopyID, MemberID, BorrowingDate, DueDate, ReturnDate, RenewalCount, AdminID)
SELECT BookCopyID, MemberID, BorrowingDate, DueDate, ReturnDate, RenewalCount, AdminID
FROM inserted;

-- 2.Update BookCopies table to set IsAvailable = 0
UPDATE bc SET bc.IsAvailable = 0
FROM BookCopies bc INNER JOIN inserted i
ON bc.CopyID = i.BookCopyID
COMMIT TRANSACTION;
END TRY
BEGIN CATCH
IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
THROW;
END CATCH;
END;";

    }
    private static string _ReservationRecordInsertAuditTrigger()
    {
        return @"
CREATE OR ALTER TRIGGER TR_ReservationRecord_Insert_Audit
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
END;";
    }
    private static string _ReservationRecordUpdateAuditTrigger()
    {
        return @"
CREATE OR ALTER TRIGGER TR_ReservationRecord_Update_Audit
ON ReservationRecords
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Insert into ReservationAudits table only if the status has changed
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
END;";
    }
    private static string _SystemSettingsUpdateAuditTrigger()
    {
        return @"
CREATE OR ALTER TRIGGER TR_SystemSettings_Update_Audit
ON SystemSettings
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
END;";
    }
    private static string _AddBookCopyInsertAuditTrigger()
    {
        return @"
CREATE OR ALTER TRIGGER TR_BookCopy_Insert_Audit
ON BookCopies
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @UserId INT = CAST(SESSION_CONTEXT(N'UserId') AS INT);

    INSERT INTO BookActivityLogs
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
        i.CopyID,                   
        NULL,                   
        NULL,                   
        NULL,                   
        3,                      
        GETDATE(),
        @UserId
    FROM inserted i;
END;";


    }



}
