namespace Domain.Entities;
public class SystemSettings : IEntity<int>
{
    public int Id { get; set; }

    public int MaxLoanDays { get; set; } = 14;
    public int MaxRenewals { get; set; } = 1;// Maximum number of times a loan can be renewed
    public int RenewalExtensionDays { get; set; } = 7;// Number of days added to the loan period when renewed

    public double FinePerDay { get; set; } = 5.00d;
    public int MaxLoansPerMember { get; set; } = 3;
    public int ReservationExpiryDays { get; set; } = 2;
    public int GracePeriodDays { get; set; } = 0;// Number of days after the due date during which no fine is charged

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
