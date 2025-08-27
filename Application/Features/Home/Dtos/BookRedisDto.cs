namespace Application.Features.Home;

internal class BookRedisDto
{
    public int Id { get; set; }
    public string TitleEN { get; set; }
    public string TitleAR { get; set; }
    public string CoverImage { get; set; }
    public bool IsFirstCategory { get; set; }
    public bool IsNewBook { get; set; }
    public bool IsMostPopular { get; set; }
    public AuthorRedisDto Author { get; set; }

}
