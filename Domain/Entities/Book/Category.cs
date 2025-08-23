
using Domain.Enums;

namespace Domain.Entities;

public class Category : IEntity<enCategory>
{
    public enCategory Id { get; set; }//CategoryID
    public string NameEN { get; set; }
    public string NameAR { get; set; }

    // Navigation Property
    public virtual ICollection<Book> Books { get; set; }
}
