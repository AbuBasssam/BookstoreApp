using Domain.Enums;

namespace Domain.Entities;

public class BorrowingRecordView : IEntity<int>
{
    public int Id { get; set; }
    public int BookCopyID { get; set; }
    public int BookId { get; set; }
    public int MemberID { get; set; }
    public string MemberName { get; set; }
    public int? ReservationRecordID { get; set; }
    public DateTime? ReservationDate { get; set; }
    public DateTime BorrowingDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public byte RenewalCount { get; set; }
    public int AdminID { get; set; }
    public string AdminName { get; set; }
    public decimal TotalFines { get; set; }
    public enBorrowingStatus BorrowingStatus { get; set; }
}
