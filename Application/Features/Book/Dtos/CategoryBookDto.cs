namespace Application.Features.Book;

public class CategoryBookDto
{
    public int BookId { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string CoverImageUrl { get; set; }
    public bool IsNewBook { get; set; }

}
