using Application.Features.Home;
using Application.Interfaces;
using Application.Models;
using Domain.AppMetaData;
using MediatR;

namespace Application.Features.Book;

public class GetBooksByCategoryHandler : IRequestHandler<GetBooksByCategoryQuery, Response<PagedResult<BookDto>>>
{
    private readonly IBookRepository _bookRepo;
    private readonly ICacheService _cacheService;
    private readonly ResponseHandler _responseHandler;

    public GetBooksByCategoryHandler(IBookRepository bookRepo, ResponseHandler responseHandler, ICacheService cacheService)
    {
        _bookRepo = bookRepo;
        _responseHandler = responseHandler;
        _cacheService = cacheService;
    }

    public async Task<Response<PagedResult<BookDto>>> Handle(GetBooksByCategoryQuery request, CancellationToken cancellationToken)
    {
        var cachedData = await _cacheService.GetAsync<HomePageRedisDto>(CacheKeys.HomePageData);
        DateOnly NewBookDateThreshold = DateOnly.Parse(cachedData!.LastUpdated.Date.ToShortDateString());

        (ICollection<BookDto> books, PagingMetadata metaData) = await _bookRepo
            .GetHomePageBookByCategoryPageAsync
            (
                NewBookDateThreshold,
                request.CategoryID,
                request.PageNumber,
                request.PageSize,
                request.Lang
            );
        var reslut = PagedResult<BookDto>.Create(books.ToList(), metaData.TotalCount, request.PageNumber, request.PageSize);
        reslut.TotalPages = metaData.TotalPages;
        return _responseHandler.Success(reslut);


    }
}
