using Application.Models;
using Domain.Enums;
using MediatR;

namespace Application.Features.Book;
public class GetBooksByCategoryQuery : IRequest<Response<PagedResult<BookDto>>>
{
    public enCategory categoryId { get; set; }
    public int pageSize { get; set; }
    public int pageNumber { get; set; }

    public string lang { get; set; }

    public GetBooksByCategoryQuery(enCategory categoryId, string lang = "en", int pageNumber = 1, int pageSize = 10)
    {
        this.categoryId = categoryId;
        this.pageNumber = pageNumber;
        this.pageSize = pageSize;
        this.lang = lang;
    }
}
