namespace Application.Features.Book.Dtos;

public record SimilarBookDto(int BookId, string Title, string Author, decimal AverageRating, string CoverImageUrl, bool IsNewBook);
