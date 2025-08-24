namespace Domain.Entities;
public class BorrowingRecord : IEntity<int>
{
    public int Id { get; set; }
    public int BookCopyID { get; set; }
    public int MemberID { get; set; }
    public int? ReservationRecordID { get; set; }
    public DateTime BorrowingDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public byte RenewalCount { get; set; }
    public int AdminID { get; set; }

    // Navigation properties
    public virtual BookCopy BookCopy { get; set; }
    public virtual User Member { get; set; }
    public virtual ReservationRecord? Reservation { get; set; }

    public virtual User Admin { get; set; }
    public virtual ICollection<Fine> Fines { get; set; }
}
