using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder;

public static class ViewsSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        var connection = context.Database.GetDbConnection();

        await SeederHelper.ExecuteSqlAsync(connection, _GetBooksView());
        await SeederHelper.ExecuteSqlAsync(connection, _GetBorrowingRecordView());
        await SeederHelper.ExecuteSqlAsync(connection, _GetReservationRecordView());

    }
    private static string _GetBooksView()
    {
        return @"CREATE OR ALTER VIEW vw_Books
AS
SELECT
	b.BookID,
	b.ISBN,
	b.TitleEN,
	b.TitleAR,
	b.DescriptionEN,
	b.DescriptionAR,
	p.PublisherID,
	p.PublisherNameEN,
	p.PublisherNameAR,
	a.AuthorID,
	a.AuthorNameEN,
	a.AuthorNameAR,
	l.LanguageNameEN AS LanguageEN,
	l.LanguageNameAR AS LanguageAR,
	c.CategoryID,
	c.CategoryNameEN,
	c.CategoryNameAR,
	b.PageCount,
	b.PublishDate,
	b.AvailabilityDate,
	b.Position,
	b.LastWaitListOpenDate,
	b.IsActive, 
	b.CoverImage,
	bb.IsBorrowable,
	br.IsReservable,
	r.Rating
	From Books b INNER JOIN Publishers
	p ON b.PublisherID = p.PublisherID INNER JOIN
	Authors a ON b.AuthorID = a.AuthorID INNER JOIN
	Languages l ON b.LanguageID = l.LanguageID INNER JOIN
	Categories c ON b.CategoryID = c.CategoryID
	OUTER APPLY dbo.fn_BookBorrowable(b.BookID) bb
	OUTER APPLY dbo.fn_BookReservable(b.BookID) br
	OUTER APPLY dbo.fn_BookRating(b.BookID) r;";
    }
    private static string _GetBorrowingRecordView()
    {
        return @"CREATE OR ALTER VIEW vw_BorrowingRecord
AS
SELECT 
    br.BorrowingRecordID,
    br.BookCopyID,
    bc.BookId,
    br.MemberID,
    m.UserName AS MemberName,
    br.ReservationRecordID,
    rr.ReservationDate,
    br.BorrowingDate,
    br.DueDate,
    br.ReturnDate,
    br.RenewalCount,
    br.AdminID,
    a.UserName AS AdminName,
	dbo.fn_TotalFineAmount(br.BorrowingRecordID)AS TotalFines,
    CASE 
        WHEN br.ReturnDate IS NOT NULL THEN 'Returned'
        WHEN br.DueDate < GETDATE() THEN 'Overdue'
        ELSE 'Active'
    END AS BorrowingStatus
FROM BorrowingRecords br
INNER JOIN BookCopies bc ON br.BookCopyID = bc.CopyID
INNER JOIN [Users] m ON br.MemberID = m.UserID AND m.RoleID=2
LEFT JOIN ReservationRecords rr ON br.ReservationRecordID = rr.ReservationRecordID
JOIN [Users] a ON br.AdminID = a.UserID AND a.RoleID=1
";

    }
    private static string _GetReservationRecordView()
    {
        return @"CREATE OR ALTER VIEW vw_ReservationRecords AS
SELECT
	rr.ReservationRecordID,
	rr.BookID,
	b.TitleEN AS BookTitleEN,
	b.TitleAR AS BookTitleAR,
	b.ISBN AS BookISBN,
	b.CoverImage AS BookCoverImage,
	rr.MemberID,
	u.UserName,
	u.Email AS MemberEmail, 
                      rr.ReservationDate,
					  CASE
						WHEN rr.Type = 1 THEN 'Waiting'
						WHEN rr.Type = 2 THEN 'Borrow'
					  END AS ReservationType, 
                      CASE
						WHEN rr.Status = 1 THEN 'Pending'
						WHEN rr.Status = 2 THEN 'Notified' WHEN rr.Status = 3 THEN 'Fulfilled' WHEN rr.Status = 4 THEN 'Expired' WHEN rr.Status = 5 THEN 'Cancelled' END AS ReservationStatus, 
                      rr.ExpirationDate,
					  CASE
						WHEN rr.Status = 1 AND rr.ExpirationDate IS NULL THEN 'Active'
						WHEN rr.Status = 2 AND (rr.Type = 1 AND rr.ExpirationDate > GETUTCDATE() AND rr.ExpirationDate <= DATEADD(Hour, 24, GETUTCDATE()) OR rr.Type = 2 AND rr.ExpirationDate > GETUTCDATE() AND rr.ExpirationDate <= DATEADD(Hour, 12, GETUTCDATE())) THEN 'Expiring Soon' WHEN rr.Status = 2 AND rr.ExpirationDate IS NOT NULL THEN 'ReadyForPickup'
					    WHEN rr.Status = 2 AND rr.ExpirationDate <= GETUTCDATE() THEN 'PickupExpired'
					    WHEN rr.Status IN (3, 4, 5) THEN 'Inactive' ELSE 'Other' END AS ReservationState,
					  CASE
						WHEN rr.ExpirationDate IS NOT NULL AND rr.ExpirationDate > GETUTCDATE() THEN DATEDIFF(SECOND, SYSUTCDATETIME(), rr.ExpirationDate)  --DATEDIFF(HOUR, GETUTCDATE(), rr.ExpirationDate) 
                        ELSE NULL END AS RemainingPickupHours,
                          (SELECT    COUNT(*) AS Expr1
                            FROM         dbo.ReservationRecords AS rr2
                            WHERE      (BookID = rr.BookID) AND (Type = 1) AND (Status = 1) AND (ReservationDate < rr.ReservationDate)) AS WaitingQueuePosition
FROM         dbo.ReservationRecords AS rr INNER JOIN
                      dbo.Books AS b ON rr.BookID = b.BookID INNER JOIN
                      dbo.Users AS u ON rr.MemberID = u.UserID AND u.RoleID = 2";

    }
}
