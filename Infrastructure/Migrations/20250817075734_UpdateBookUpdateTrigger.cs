using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookUpdateTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
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
        INSERT INTO BookAuditLogs
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
    INSERT INTO BookAuditLogs
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
    INSERT INTO BookAuditLogs
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
        5, -- UpdateBookInfo
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

END;");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS TR_Book_Update_Audit ON Books;
            ");

        }
    }
}
