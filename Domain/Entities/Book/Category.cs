
using Domain.Enums;

namespace Domain.Entities;

public class Category : IEntity<enCategory>
{
    public enCategory Id { get; set; }//CategoryID
    public required string NameEN { get; set; }
    public required string NameAR { get; set; }

    // Navigation Property
    public virtual ICollection<Book> Books { get; set; }
}
