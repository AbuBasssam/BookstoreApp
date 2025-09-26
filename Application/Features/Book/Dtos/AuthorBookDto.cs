namespace Application.Features.Book.Dtos;

public record AuthorBookDto(int BookId, string Title, string PublicationYear, double AverageRating, string CoverImageUrl, bool IsNewBook);
