namespace Domain.Entities;

public class ReservationRecord : IEntity<int>
{

    public int Id { get; set; }


    public int BookID { get; set; }


    public int MemberID { get; set; }


    public DateTime ReservationDate { get; set; } = DateTime.UtcNow;


    public bool IsCancelled { get; set; }


    // Navigation properties
    public virtual Book Book { get; set; }
    public virtual User Member { get; set; }
}
