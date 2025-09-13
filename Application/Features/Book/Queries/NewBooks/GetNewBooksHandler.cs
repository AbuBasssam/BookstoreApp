using Application.Features.Home;
using Application.Interfaces;
using Application.Models;
using Domain.AppMetaData;
using Domain.HelperClasses;
using MediatR;

namespace Application.Features.Book;

public class GetNewBooksHandler : IRequestHandler<GetNewBooksQuery, Response<PagedResult<BookDto>>>
{
    private readonly IBookRepository _bookRepo;
    private readonly ICacheService _cacheService;
    private readonly ResponseHandler _responseHandler;
    private readonly HomePageSettings _homePageSettings;

    public GetNewBooksHandler(IBookRepository bookRepo, ICacheService cacheService, ResponseHandler responseHandler, HomePageSettings homePageSettings)
    {
        _bookRepo = bookRepo;
        _cacheService = cacheService;
        _responseHandler = responseHandler;
        _homePageSettings = homePageSettings;
    }



    public async Task<Response<PagedResult<BookDto>>> Handle(GetNewBooksQuery request, CancellationToken cancellationToken)
    {
        var cachedData = await _cacheService.GetAsync<HomePageRedisDto>(CacheKeys.HomePageData);
        DateOnly NewBookDateThreshold = DateOnly.Parse(cachedData!.LastUpdated.Date.ToShortDateString());
        var pageInfo = new LocalizePaginationInfo { PageNumber = request.PageNumber, PageSize = request.PageSize, Lang = request.Lang };
        var settings = new NewBookSetting { NewBooksDateThreshold = NewBookDateThreshold, NewBooksDaysThreshold = _homePageSettings.NewBooksDaysThreshold };

        (ICollection<BookDto> books, PagingMetadata metaData) = await _bookRepo.GetHomePageNewestBooksPageAsync(pageInfo, settings);

        var reslut = PagedResult<BookDto>.Create(books.ToList(), metaData.TotalCount, request.PageNumber, request.PageSize);

        reslut.TotalPages = metaData.TotalPages;

        return _responseHandler.Success(reslut);


    }


}
