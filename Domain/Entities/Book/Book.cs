using Domain.Enums;

namespace Domain.Entities;
public class Book : IEntity<int>
{
    public int Id { get; set; }
    public string ISBN { get; set; }
    public string TitleEN { get; set; }
    public string TitleAR { get; set; }
    public string DescriptionEN { get; set; }
    public string DescriptionAR { get; set; }
    public int PublisherID { get; set; }
    public int AuthorID { get; set; }
    public int LanguageID { get; set; }
    public enCategory CategoryID { get; set; }
    public short PageCount { get; set; }
    public DateTime PublishDate { get; set; }
    public DateOnly AvailabilityDate { get; set; }
    public string Position { get; set; }
    public DateTime? LastWaitListOpenDate { get; set; }
    public bool IsActive { get; set; }
    public string CoverImage { get; set; }

    // Navigation Properties
    public virtual Category Category { get; set; }
    public virtual Language Language { get; set; }
    public virtual Publisher Publisher { get; set; }
    public virtual Author Author { get; set; }
    public virtual ICollection<BookCopy> Copies { get; set; }
    public virtual ICollection<ReservationRecord> Reservations { get; set; }
    public virtual ICollection<BookRating> Ratings { get; set; }
    public virtual ICollection<BookActivityLog> AuditLogs { get; set; }
}
