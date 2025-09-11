using Application.Models;
using Domain.Enums;
using MediatR;

namespace Application.Features.Book;
public class GetBooksByCategoryQuery : IRequest<Response<PagedResult<CategoryBookDto>>>
{
    public enCategory categoryId { get; set; }
    public int pageSize { get; set; } = 10;
    public int pageNumber { get; set; } = 1;

    public string lang = "en";

}
