using Application.Models;
using MediatR;

namespace Application.Features.Home;
public record GetHomePageDataQuery(string token, string langCode) : IRequest<Response<HomePageResponseDto>>;
