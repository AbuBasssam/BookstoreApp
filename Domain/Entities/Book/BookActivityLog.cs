using Domain.Enums;

namespace Domain.Entities;

public class BookActivityLog : IEntity<int>
{

    public int Id { get; set; }
    public int BookID { get; set; }
    public int? CopyID { get; set; }
    public string? UpdatedFieldName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public enBookActionType ActionType { get; set; }
    public DateTime ActionDate { get; set; }
    public int? ByUserID { get; set; }

    // Navigation properties
    public virtual Book Book { get; set; }
    public virtual BookCopy? Copy { get; set; }
    public virtual User? User { get; set; }
}