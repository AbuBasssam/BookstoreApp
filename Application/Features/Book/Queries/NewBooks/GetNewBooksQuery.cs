using Application.Abstracts;
using Application.Models;
using MediatR;

namespace Application.Features.Book;
public class GetNewBooksQuery : LocalizePaginationQuery, IRequest<Response<PagedResult<BookDto>>>
{
    public GetNewBooksQuery(int pageNumber, int pageSize, string lang)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        Lang = lang;

    }
}
