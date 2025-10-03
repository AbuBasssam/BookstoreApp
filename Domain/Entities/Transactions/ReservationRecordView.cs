namespace Domain.Entities;

public class ReservationRecordView : IEntity<int>
{
    public int Id { get; set; }
    public int BookID { get; set; }
    public string BookTitleEN { get; set; }
    public string BookTitleAR { get; set; }
    public string BookISBN { get; set; }
    public string BookCoverImage { get; set; }
    public int MemberID { get; set; }
    public string UserName { get; set; }
    public string MemberEmail { get; set; }
    public DateTime ReservationDate { get; set; }
    public string ReservationType { get; set; }
    public string ReservationStatus { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string ReservationState { get; set; }
    public int? HoursUntilExpiration { get; set; }
    public int? RemainingPickupHours { get; set; }
    public int WaitingQueuePosition { get; set; }
}