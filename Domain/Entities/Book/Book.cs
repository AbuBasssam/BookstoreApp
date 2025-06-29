using Domain.Enums;

namespace Domain.Entities;
public class Book : IEntity<int>
{
    public int Id { get; set; } // BookID
    public required string TitleEN { get; set; }
    public required string TitleAR { get; set; }

    public required DateTime PublishDate { get; set; }
    public required string ISBN { get; set; }
    public enCategory CategoryID { get; set; }
    public int AuthorID { get; set; }
    public int NumberOfCopies => Copies?.Count ?? 0;

    public string ImageUrl { get; set; }
    // Navigation Properties
    public virtual Category Category { get; set; }
    public virtual Author Author { get; set; }

    public virtual ICollection<BookCopy> Copies { get; set; }
    public virtual ICollection<ReservationRecord> Reservations { get; set; }
}
