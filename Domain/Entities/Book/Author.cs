namespace Domain.Entities;

public class Author : IEntity<int>
{
    public int Id { get; set; }//AuthorID
    public string NameEN { get; set; }
    public string NameAR { get; set; }
    public DateTime? BirthDate { get; set; }
    public string Bio { get; set; }
    // Navigation Properties
    public virtual ICollection<Book> Books { get; set; }
}