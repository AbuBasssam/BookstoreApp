using Application.Models;
using MediatR;

namespace Application.Features.Home;
public record GetHomePageDataQuery() : IRequest<Response<HomePageResponseDto>>;
