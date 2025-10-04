namespace Application.Features.Book.Dtos;

public record AuthorBookDto(int BookId, string Title, int PublicationYear, decimal? AverageRating, string CoverImageUrl, bool IsNewBook);
