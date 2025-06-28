using Domain.Enums;

namespace Domain.Entities;
public class BorrowingRecord : IEntity<int>
{
    public int Id { get; set; }


    public int BookCopyID { get; set; }


    public int MemberID { get; set; }

    public DateTime InitialBorrowingDate { get; set; } = DateTime.UtcNow;

    public DateTime? ActualBorrowingDate { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public byte RenewalCount { get; set; }


    public enBorrowingStatus Status { get; set; } // pending, active, returned, overdue, cancelled


    public int? AdminID { get; set; }// to handle manual Borrowing by admin


    // Navigation properties
    public virtual BookCopy BookCopy { get; set; }
    public virtual User Member { get; set; }
    public virtual User Admin { get; set; }
    public virtual ICollection<Fine> Fines { get; set; }
}
