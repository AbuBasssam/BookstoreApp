using Domain.Enums;

namespace Domain.Entities;

public class BorrowingAudit : IEntity<int>
{
    public int Id { get; set; }
    public int BorrowingID { get; set; }
    public BorrowAction Action { get; set; }
    public int UserID { get; set; }
    public DateTime Timestamp { get; set; }
    public DateTime? OldDueDate { get; set; }
    public DateTime? NewDueDate { get; set; }

    public virtual BorrowingRecord Borrowing { get; set; }
    public virtual User User { get; set; }
}