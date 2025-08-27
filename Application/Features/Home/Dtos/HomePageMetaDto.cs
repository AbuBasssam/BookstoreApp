namespace Application.Features.Home;

//public record HomePageMetaDto(int FirstCategoryPageSize, int TotalFirstCategoryBooks, int NewBooksPageSize, int TotalNewBooks);
public record HomePageMetaDto
{
    public int FirstCategoryPageSize { get; set; }
    public int TotalFirstCategoryBooks { get; set; }
    public int NewBooksPageSize { get; set; }
    public int TotalNewBooks { get; set; }
}
