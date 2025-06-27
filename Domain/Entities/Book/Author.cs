namespace Domain.Entities;

public class Author : IEntity<int>
{
    public int Id { get; set; }//AuthorID
    public string AuthorNameEN { get; set; }
    public string AuthorNameAR { get; set; }
    public DateTime? BirthDate { get; set; }
    public string Bio { get; set; }
    // Navigation Properties
    public virtual ICollection<Book> Books { get; set; }
}