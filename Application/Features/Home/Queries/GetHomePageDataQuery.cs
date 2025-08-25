using MediatR;

namespace Application.Features.Home;
public record GetHomePageDataQuery() : IRequest<HomePageResponseDto>;