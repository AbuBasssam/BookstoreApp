using Domain.Enums;

namespace Domain.Entities;
public class Book : IEntity<int>
{
    public int Id { get; set; }
    public required string ISBN { get; set; }
    public required string TitleEN { get; set; }
    public required string TitleAR { get; set; }
    public required string DescriptionEN { get; set; }
    public required string DescriptionAR { get; set; }
    public required int PublisherID { get; set; }
    public required int AuthorID { get; set; }
    public required int LanguageID { get; set; }
    public required enCategory CategoryID { get; set; }
    public required short PageCount { get; set; }
    public required DateTime PublishDate { get; set; }
    public required DateOnly AvailabilityDate { get; set; }
    public required string Position { get; set; }
    public DateTime? LastReservationOpenDate { get; set; }
    public required string CoverImage { get; set; }
    public int NumberOfCopies => Copies?.Count ?? 0;

    // Navigation Properties
    public virtual Category Category { get; set; }
    public virtual Language Language { get; set; }
    public virtual Publisher Publisher { get; set; }
    public virtual Author Author { get; set; }
    public virtual ICollection<BookCopy> Copies { get; set; }
    public virtual ICollection<ReservationRecord> Reservations { get; set; }
    public virtual ICollection<BookRating> Ratings { get; set; }
    public virtual ICollection<BookAuditLog> AuditLogs { get; set; }
}
