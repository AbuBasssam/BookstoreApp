using Application.Features.Home;
using Application.Interfaces;
using Application.Models;
using Domain.AppMetaData;
using MediatR;

namespace Application.Features.Book;

public class GetBooksByCategoryHandler : IRequestHandler<GetBooksByCategoryQuery, Response<PagedResult<CategoryBookDto>>>
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

    public async Task<Response<PagedResult<CategoryBookDto>>> Handle(GetBooksByCategoryQuery request, CancellationToken cancellationToken)
    {
        var cachedData = await _cacheService.GetAsync<HomePageRedisDto>(CacheKeys.HomePageData);
        DateOnly NewBookDateThreshold = DateOnly.Parse(cachedData!.LastUpdated.Date.ToShortDateString());

        (ICollection<CategoryBookDto> books, PagingMetadata metaData) = await _bookRepo
            .GetHomeBookPageDataByCategory
            (
                NewBookDateThreshold,
                request.categoryId,
                request.pageNumber,
                request.pageSize,
                request.lang
            );
        var reslut = PagedResult<CategoryBookDto>.Create(books.ToList(), metaData.TotalCount, request.pageNumber, request.pageSize);
        reslut.TotalPages = metaData.TotalPages;
        return _responseHandler.Success(reslut);


    }
}
