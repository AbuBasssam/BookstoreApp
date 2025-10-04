using Application.Features.Book.Dtos;
using Application.Models;
using MediatR;

namespace Application.Features.Book.Queries;
public class GetBookDetailsQuery : IRequest<Response<BookDetailsDto>>
{
    public int Id { get; set; }
    public string LangCode { get; set; }
    public string Token { get; set; }
    public GetBookDetailsQuery(int id, string langCode, string token)
    {
        Id = id;
        LangCode = langCode;
        Token = token;
    }
}
