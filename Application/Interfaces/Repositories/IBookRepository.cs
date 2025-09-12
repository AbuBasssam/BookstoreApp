using Application.Features.Book;
using Application.Models;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces;

public interface IBookRepository : IGenericRepository<Book, int>
{
    Task<(ICollection<CategoryBookDto>, PagingMetadata)> GetHomeBookPageDataByCategory(DateOnly NewBookDateThreshold, enCategory categoryId, int pageNumber = 1, int pageSize = 10, string LangCode = "en");

}
