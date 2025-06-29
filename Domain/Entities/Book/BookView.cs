using Domain.Enums;

namespace Domain.Entities;

public class BookView: IEntity<int>
{
    public int Id { get; set; }
    public string TitleEN { get; set; }
    public string TitleAR { get; set; }
    public DateTime PublishDate { get; set; }
    public string ISBN { get; set; }
    public enCategory CategoryID { get; set; }
    public int AuthorID { get; set; }
    public string ImageUrl { get; set; }
    public bool IsAvailable { get; set; } // True if ANY copy is available
}