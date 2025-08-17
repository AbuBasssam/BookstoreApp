namespace Domain.Entities;

public class BookView : IEntity<int>
{
    public int Id { get; set; }
    public string ISBN { get; set; } = null!;
    public string TitleEN { get; set; } = null!;
    public string TitleAR { get; set; } = null!;
    public string DescriptionEN { get; set; } = null!;
    public string DescriptionAR { get; set; } = null!;
    public string PublisherEN { get; set; } = null!;
    public string PublisherAR { get; set; } = null!;
    public string AuthorEN { get; set; } = null!;
    public string AuthorAR { get; set; } = null!;
    public string LanguageEN { get; set; } = null!;
    public string LanguageAR { get; set; } = null!;
    public string CategoryEN { get; set; } = null!;
    public string CategoryAR { get; set; } = null!;
    public short PageCount { get; set; }
    public DateTime PublishDate { get; set; }
    public DateOnly AvailabilityDate { get; set; }
    public string Position { get; set; } = null!;
    public DateTime? LastWaitListOpenDate { get; set; }
    public bool IsActive { get; set; }
    public string CoverImage { get; set; } = null!;
    public bool IsBorrowable { get; set; }
    public bool IsReservable { get; set; }
    public decimal Rating { get; set; }
}