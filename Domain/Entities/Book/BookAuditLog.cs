using Domain.Enums;

namespace Domain.Entities;

public class BookAuditLog : IEntity<int>
{
       
        public int Id { get; set; }
        public int BookId { get; set; }
        public int? CopyId { get; set; }
        public string? UpdatedFieldName { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public BookActionType ActionType { get; set; }
        public DateTime ActionDate { get; set; }
        public int ByUserId { get; set; }

        // Navigation properties
        public virtual Book Book { get; set; }
        public virtual BookCopy? Copy { get; set; }
        public virtual User User { get; set; }
    }