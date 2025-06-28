namespace Domain.Entities;

public class Fine : IEntity<int>
{

    public int Id { get; set; }


    public int BorrowingID { get; set; }

    public byte TotalLateDays { get; set; }


    public double Amount { get; set; }


    public DateTime IssueDate { get; set; } = DateTime.UtcNow;

    public DateTime? PaymentDate { get; set; }


    public bool IsPaid { get; set; }

    // Navigation property
    public virtual BorrowingRecord Borrowing { get; set; }
}