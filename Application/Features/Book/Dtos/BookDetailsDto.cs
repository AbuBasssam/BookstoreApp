using Application.Features.Book.Enums;

namespace Application.Features.Book.Dtos;
public record BookDetailsDto(
    int bookId, string title, string author, string description,
    string ISBN, enBookState bookState, string CoverImageUrl, bool isNewBook,
    string publisher, string publicationYear, short pages, string language,
    string category, double averageRating, double userRating, int totalReaders,
    List<SimilarBookDto> similarBooks, List<AuthorBookDto> relatedBooks);
