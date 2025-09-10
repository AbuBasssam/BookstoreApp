using Application.Features.Home;
using Application.Models;

namespace Application.Interfaces;

public interface IHomePageService
{
    /// <summary>
    /// Get books for a specific category with pagination
    /// </summary>
    Task<PagedResult<HomePageBookDto>> GetCategoryBooksAsync(int categoryId, int page, int pageSize, string language);

    /// <summary>
    /// Get new books with pagination
    /// </summary>
    Task<PagedResult<HomePageBookDto>> GetNewBooksAsync(int page, int pageSize, string language);

}