using Domain.Enums;

namespace Domain.Entities;
public class Notification : IEntity<int>
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int UserDeviceId { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public NotificationType NotificationType { get; set; }
    public DateTime SentDate { get; set; }
    public bool IsRead { get; set; }
    public int? RelatedEntityId { get; set; }
    public NotificationEntityType? EntityType { get; set; }

    // Navigation properties
    public virtual User User { get; set; }
    public virtual UserDevice UserDevice { get; set; }
}
