namespace Application.Features.Home;

public record HomePageBookDto(int BookId, string Title, string Author, string CoverImageUrl, bool IsFirstCategory, bool IsNewBook, bool IsMostPopular);
