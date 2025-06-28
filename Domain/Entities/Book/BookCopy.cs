namespace Domain.Entities;

public class BookCopy : IEntity<int>
{
    public int Id { get; set; }// CopyID
    public int BookID { get; set; }
    public bool IsAvailable { get; set; } = true;

    // Navigation Properties
    public virtual Book Book { get; set; }
    public virtual ICollection<BorrowingRecord> BorrowingRecords { get; set; }
}
