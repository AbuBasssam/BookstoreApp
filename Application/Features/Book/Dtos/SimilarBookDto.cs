namespace Application.Features.Book.Dtos;

public record SimilarBookDto(int BookId, string Title, string Author, double AverageRating, string CoverImageUrl, bool IsNewBook);
