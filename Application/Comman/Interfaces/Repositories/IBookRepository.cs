using Application.Features.Book;
using Application.Models;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces;

public interface IBookRepository : IGenericRepository<Book, int>
{
    Task<(ICollection<BookDto>, PagingMetadata)> GetHomePageBookByCategoryPageAsync(DateOnly NewBookDateThreshold, enCategory categoryId, int pageNumber = 1, int pageSize = 10, string LangCode = "en");
    Task<(ICollection<BookDto>, PagingMetadata)> GetHomePageNewestBooksPageAsync(LocalizePaginationInfo paginationInfo, NewBookSetting newBookSetting);
}
