namespace Domain.Entities;
public class SystemSettings
{
    public int MaxLoanDays { get; set; } = 30;
    public int MaxRenewals { get; set; } = 2;
    public int RenewalExtensionDays { get; set; } = 7;
    public decimal FinePerDay { get; set; } = 5.00m;
    public int MaxLoansPerMember { get; set; } = 5;
    public byte ReservationExpiryDays { get; set; } = 1;
    public byte PickupExpiryHours { get; set; } = 48;

}
