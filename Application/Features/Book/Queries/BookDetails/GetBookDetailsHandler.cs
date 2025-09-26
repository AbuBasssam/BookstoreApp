using Application.Features.Book.Dtos;
using Application.Models;
using MediatR;

namespace Application.Features.Book.Queries;

public class GetBookDetailsHandler : IRequestHandler<GetBookDetailsQuery, Response<BookDetailsDto>>
{
    public async Task<Response<BookDetailsDto>> Handle(GetBookDetailsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
