using Domain.Enums;

namespace Domain.Entities;

public class ReservationRecord : IEntity<int>
{

    public int Id { get; set; }
    public int BookID { get; set; }
    public int MemberID { get; set; }
    public DateTime ReservationDate { get; set; } = DateTime.UtcNow;
    public enReservationType Type { get; set; }
    public enReservationStatus Status { get; set; } = enReservationStatus.Pending;
    public DateTime? ExpirationDate { get; set; }

    // Navigation properties
    public virtual Book Book { get; set; }
    public virtual User Member { get; set; }
}
