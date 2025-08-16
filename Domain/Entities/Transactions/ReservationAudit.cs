using Domain.Enums;

namespace Domain.Entities;

public class ReservationAudit : IEntity<int>
{
    public int Id { get; set; }
    public int ReservationID { get; set; }
    public ReservationAction Action { get; set; }
    public int? BorrowingID { get; set; }
    public int? UserID { get; set; }
    public DateTime Timestamp { get; set; }

    public virtual ReservationRecord Reservation { get; set; }
    public virtual BorrowingRecord? Borrowing { get; set; }
    public virtual User? User { get; set; }
}