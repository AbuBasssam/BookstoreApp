using Application.Models;
using MediatR;

namespace Application.Features.Home;
public record GetHomePageDataQuery(string langCode) : IRequest<Response<HomePageResponseDto>>;
