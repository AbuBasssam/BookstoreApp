using Application.Interfaces;
using Application.Models;
using Domain.AppMetaData;
using Domain.Entities;
using Domain.HelperClasses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Application.Features.Home;

public class GetHomePageDataQueryHandler : IRequestHandler<GetHomePageDataQuery, Response<HomePageResponseDto>>
{
    private readonly ICacheService _cacheService;
    private readonly IAuthService _authServices;
    private readonly IGenericRepository<Notification, int> _notificationRepository;
    private readonly ResponseHandler _responseHandler;



    public GetHomePageDataQueryHandler(ICacheService cacheService, HomePageSettings settings, ResponseHandler responseHandler,
        IAuthService authServices, IGenericRepository<Notification, int> notificationRepository)
    {
        _cacheService = cacheService;
        _responseHandler = responseHandler;
        _authServices = authServices;
        _notificationRepository = notificationRepository;
    }

    public async Task<Response<HomePageResponseDto>> Handle(GetHomePageDataQuery request, CancellationToken cancellationToken)
    {
        // Get cached data
        var cachedData = await _cacheService.GetAsync<HomePageRedisDto>(CacheKeys.HomePageData);

        if (cachedData == null)
        {
            return new Response<HomePageResponseDto>
            {
                Succeeded = false,
                StatusCode = HttpStatusCode.NotFound,
                Message = "Homepage data not found in cache. Please wait for the background service to seed the data.",
                Errors = new List<string> { "Cache data not available" }
            };
        }

        // Convert cached data to response DTOs based on language
        var categories = _MapCategories(cachedData.CategoriesData, request.langCode.ToLower());
        var books = _MapBooks(cachedData.PageData, request.langCode.ToLower());

        /*//Validate token and get user ID
        (JwtSecurityToken obj, Exception ex) = _authServices.GetJwtAccessTokenObjFromAccessTokenString(request.token);
        if (ex != null)
        {
            return _responseHandler.BadRequest<HomePageResponseDto>(ex.Message);
        }
        (int userId, Exception e) = _authServices.GetUserIdFromJwtAccessTokenObj(obj!);
        if (e != null)
        {
            return _responseHandler.BadRequest<HomePageResponseDto>(e.Message);
        }*/

        int notificationsCount = await _notificationRepository.GetTableNoTracking().CountAsync(n => n.UserDevice.UserId == 2 && !n.IsRead);


        var responseDto = new HomePageResponseDto(notificationsCount, categories, books);
        return _responseHandler.Success<HomePageResponseDto>(responseDto, cachedData.MetaData);

        return new Response<HomePageResponseDto>
        {
            Succeeded = true,
            StatusCode = HttpStatusCode.OK,
            Data = responseDto,
            Meta = _MapMeta(cachedData.MetaData),
            Message = "Homepage data retrieved successfully"
        };
    }
    private List<CategoryDto> _MapCategories(List<Category> categories, string language)
    {
        return categories.Select(c => new CategoryDto(
            CategoryId: (int)c.Id,
            Name: language == "ar" ? c.NameAR : c.NameEN
        )).ToList();
    }

    private List<HomePageBookDto> _MapBooks(List<BookRedisDto> books, string language)
    {
        return books.Select(b => new HomePageBookDto(
            BookId: b.Id,
            Title: language == "ar" ? b.TitleAR : b.TitleEN,
            Author: language == "ar" ? b.Author.NameAR : b.Author.NameEN,
            CoverImageUrl: b.CoverImage,
            IsFirstCategory: b.IsFirstCategory,
            IsNewBook: b.IsNewBook,
            IsMostPopular: b.IsMostPopular
        )).ToList();
    }
    private Dictionary<string, int> _MapMeta(HomePageMetaDto meta)
    {
        return new Dictionary<string, int>
        {
            { "FirstCategoryPageSize", meta.FirstCategoryPageSize },
            { "TotalFirstCategoryBooks", meta.TotalFirstCategoryBooks },
            { "NewBooksPageSize", meta.NewBooksPageSize },
            { "TotalNewBooks", meta.TotalNewBooks }
        };
    }
}