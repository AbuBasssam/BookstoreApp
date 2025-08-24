using Domain.Enums;

namespace Domain.Entities;
public class Notification : IEntity<int>
{
    public int Id { get; set; }
    public int UserDeviceID { get; set; }
    public string TitleEN { get; set; }
    public string TitleAR { get; set; }
    public string MessageEN { get; set; }
    public string MessageAR { get; set; }
    public enNotificationType NotificationType { get; set; }
    public DateTime SentDate { get; set; }
    public bool IsRead { get; set; }
    // Navigation Properties
    public virtual UserDevice UserDevice { get; set; }

}
