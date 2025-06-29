using Domain.Enums;

namespace Domain.Entities;

public class UserDevice : IEntity<int>
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; }
    public string DeviceName { get; set; }
    public enPlatform Platform { get; set; } // 'android', 'ios', 'web'
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation property
    public virtual User User { get; set; }
}