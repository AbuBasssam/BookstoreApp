using Application.Features.Book.Dtos;
using Application.Models;
using MediatR;

namespace Application.Features.Book.Queries;
public class GetBookDetailsQuery : IRequest<Response<BookDetailsDto>>
{
    public int Id { get; set; }
    public string LangCode { get; set; }
    public GetBookDetailsQuery(int Id, string langCode)
    {
        this.Id = Id;
        this.LangCode = langCode;
    }
}
