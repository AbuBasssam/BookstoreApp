namespace Domain.Entities;

public class ReservationNotification
{
    public int Id { get; set; }
    public int NotificationId { get; set; }
    public int ReservationId { get; set; }

    public virtual Notification Notification { get; set; }
    public virtual ReservationRecord Reservation { get; set; }
}