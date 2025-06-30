using Domain.Enums;

namespace Domain.Entities;

public class Otp : IEntity<int>
{
    public int Id { get; set; }

    public string Code { get; set; }

    public enOtpType Type { get; set; }

    public DateTime CreationTime { get; set; }
    public DateTime ExpirationTime { get; set; }

    public int UserID { get; set; }

    // Navigation property
    public virtual User User { get; set; }




}


