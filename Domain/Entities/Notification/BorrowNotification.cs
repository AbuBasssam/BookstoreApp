namespace Domain.Entities;

public class BorrowNotification
{
    public int Id { get; set; }
    public int NotificationID { get; set; }
    public int BorrowID { get; set; }

    public virtual Notification Notification { get; set; }
    public virtual BorrowingRecord Borrow { get; set; }
}