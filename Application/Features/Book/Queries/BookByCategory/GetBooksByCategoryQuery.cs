using Application.Models;
using Domain.Enums;
using MediatR;

namespace Application.Features.Book;
public class GetBooksByCategoryQuery : LocalizePaginationInfo, IRequest<Response<PagedResult<BookDto>>>
{
    public enCategory CategoryID { get; set; }

    public GetBooksByCategoryQuery(enCategory categoryId, int pageNumber = 1, int pageSize = 10, string lang = "en")
    {
        this.CategoryID = categoryId;
        this.PageNumber = pageNumber;
        this.PageSize = pageSize;
        this.Lang = lang;
    }
}
