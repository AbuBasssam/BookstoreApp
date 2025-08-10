namespace Domain.Entities;

public class BookRating : IEntity<int>
{
    public int Id { get; set; }

    public int BookID { get; set; }

    public int UserID { get; set; }

    public byte Rating { get; set; }

    // Navigation properties
    public virtual Book Book { get; set; }
    public virtual User User { get; set; }
}