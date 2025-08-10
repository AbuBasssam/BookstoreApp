namespace Domain.Entities;

public class ReservationNotification
{
    public int Id { get; set; }
    public int NotificationID { get; set; }
    public int ReservationID { get; set; }

    public virtual Notification Notification { get; set; }
    public virtual ReservationRecord Reservation { get; set; }
}