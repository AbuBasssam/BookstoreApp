using Application.Interfaces;
using Application.Models;
using Domain.HelperClasses;
using MediatR;

namespace Application.Features.Home;

public class GetHomePageDataQueryHandler : IRequestHandler<GetHomePageDataQuery, Response<HomePageResponseDto>>
{
    private readonly ICacheService _cacheService;
    private readonly HomePageSettings _settings;

    public GetHomePageDataQueryHandler(ICacheService cacheService, HomePageSettings settings)
    {
        _cacheService = cacheService;
        _settings = settings;
    }

    public async Task<Response<HomePageResponseDto>> Handle(GetHomePageDataQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();

    }
}