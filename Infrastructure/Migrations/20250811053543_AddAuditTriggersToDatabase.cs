using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditTriggersToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            // Helper procedure to set user context
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_SetAuditContext
                    @UserId INT
                AS
                BEGIN
                    EXEC sp_set_session_context @key = N'UserId', @value = @UserId;
                END;"
            );
            // Trigger for Book Updates
            migrationBuilder.Sql(@"CREATE TRIGGER TR_Book_Update_Audit
ON Books
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @UserId INT =ISNULL(CAST(SESSION_CONTEXT(N'UserId') AS INT), NULL);
	 IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN deleted d ON i.BookID = d.BookID
        WHERE i.LastReservationOpenDate <> d.LastReservationOpenDate)
		 BEGIN
        -- سجل التغيير الخاص بفتح/إغلاق الانتظار فقط
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
            'LastReservationOpenDate',
            CONVERT(NVARCHAR(30), d.LastReservationOpenDate, 126),
            CONVERT(NVARCHAR(30), i.LastReservationOpenDate, 126),
            CASE
                WHEN (d.LastReservationOpenDate IS NULL AND i.LastReservationOpenDate IS NOT NULL OR d.LastReservationOpenDate IS NOT NULL AND i.LastReservationOpenDate > d.LastReservationOpenDate)
                   
                    THEN 1 -- OpenWaitList(first time)/OpenWaitList (تحديث لاحق)
                ELSE 2 -- CloseWaitList
            END,
            GETDATE(),
            NULL
        FROM inserted i
        INNER JOIN deleted d ON i.BookID = d.BookID;

        -- الخروج من التريجر بعد تسجيل تغيير الانتظار فقط
        RETURN;
    END
 INSERT INTO BookAuditLogs (BookID, UpdatedFieldName, OldValue, NewValue, ActionType, ActionDate, ByUserID)
    SELECT
        changes.BookID,
        changes.FieldName,
        changes.OldValue,
        changes.NewValue,
        5, -- UpdateBookInfo
        GETDATE(),
        @UserId
    FROM
    (
        SELECT
            i.BookID,
            'ISBN' AS FieldName, CAST(d.ISBN AS NVARCHAR(MAX)) AS OldValue, CAST(i.ISBN AS NVARCHAR(MAX)) AS NewValue
        FROM inserted i
        JOIN deleted d ON i.BookID = d.BookID
        WHERE i.ISBN <> d.ISBN

        UNION ALL
        SELECT
            i.BookID,
            'TitleEN', d.TitleEN, i.TitleEN
        FROM inserted i
        JOIN deleted d ON i.BookID = d.BookID
        WHERE i.TitleEN <> d.TitleEN

        UNION ALL
        SELECT
            i.BookID,
            'TitleAR', d.TitleAR, i.TitleAR
        FROM inserted i
        JOIN deleted d ON i.BookID = d.BookID
        WHERE i.TitleAR <> d.TitleAR

        UNION ALL
        SELECT
            i.BookID,
            'DescriptionEN', d.DescriptionEN, i.DescriptionEN
        FROM inserted i
        JOIN deleted d ON i.BookID = d.BookID
        WHERE i.DescriptionEN <> d.DescriptionEN

        UNION ALL
        SELECT
            i.BookID,
            'DescriptionAR', d.DescriptionAR, i.DescriptionAR
        FROM inserted i
        JOIN deleted d ON i.BookID = d.BookID
        WHERE i.DescriptionAR <> d.DescriptionAR

        UNION ALL
        SELECT
            i.BookID,
            'PublisherID', CAST(d.PublisherID AS NVARCHAR(50)), CAST(i.PublisherID AS NVARCHAR(50))
        FROM inserted i
        JOIN deleted d ON i.BookID = d.BookID
        WHERE i.PublisherID <> d.PublisherID

        UNION ALL
        SELECT
            i.BookID,
            'AuthorID', CAST(d.AuthorID AS NVARCHAR(50)), CAST(i.AuthorID AS NVARCHAR(50))
        FROM inserted i
        JOIN deleted d ON i.BookID = d.BookID
        WHERE i.AuthorID <> d.AuthorID

        UNION ALL
        SELECT
            i.BookID,
            'LanguageID', CAST(d.LanguageID AS NVARCHAR(50)), CAST(i.LanguageID AS NVARCHAR(50))
        FROM inserted i
        JOIN deleted d ON i.BookID = d.BookID
        WHERE i.LanguageID <> d.LanguageID

        UNION ALL
        SELECT
            i.BookID,
            'CategoryID', CAST(d.CategoryID AS NVARCHAR(50)), CAST(i.CategoryID AS NVARCHAR(50))
        FROM inserted i
        JOIN deleted d ON i.BookID = d.BookID
        WHERE i.CategoryID <> d.CategoryID

        UNION ALL
        SELECT
            i.BookID,
            'PageCount', CAST(d.PageCount AS NVARCHAR(50)), CAST(i.PageCount AS NVARCHAR(50))
        FROM inserted i
        JOIN deleted d ON i.BookID = d.BookID
        WHERE i.PageCount <> d.PageCount

        UNION ALL
        SELECT
            i.BookID,
            'PublishDate', CONVERT(NVARCHAR(30), d.PublishDate, 126), CONVERT(NVARCHAR(30), i.PublishDate, 126)
        FROM inserted i
        JOIN deleted d ON i.BookID = d.BookID
        WHERE i.PublishDate <> d.PublishDate

        UNION ALL
        SELECT
            i.BookID,
            'AvailabilityDate', CONVERT(NVARCHAR(30), d.AvailabilityDate, 126), CONVERT(NVARCHAR(30), i.AvailabilityDate, 126)
        FROM inserted i
        JOIN deleted d ON i.BookID = d.BookID
        WHERE i.AvailabilityDate <> d.AvailabilityDate

        UNION ALL
        SELECT
            i.BookID,
            'Position', d.Position, i.Position
        FROM inserted i
        JOIN deleted d ON i.BookID = d.BookID
        WHERE i.Position <> d.Position

        UNION ALL
        SELECT
            i.BookID,
            'CoverImage', d.CoverImage, i.CoverImage
        FROM inserted i
        JOIN deleted d ON i.BookID = d.BookID
        WHERE i.CoverImage <> d.CoverImage
    ) AS changes;


	END;");

            // update LastReservationOpenDate for Nullable value
            migrationBuilder.AlterColumn<DateTime?>(
                name: "LastReservationOpenDate",
                table: "Books",
                type: "datetime2(7)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: false

            );

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the trigger for Book Updates
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TR_Book_Update_Audit ON Books;");
            // Remove the stored procedure for setting audit context
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_SetAuditContext;");

            // Revert LastReservationOpenDate to non-nullable
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastReservationOpenDate",
                table: "Books",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateTime?),
                oldNullable: true
            );

        }
    }
}
