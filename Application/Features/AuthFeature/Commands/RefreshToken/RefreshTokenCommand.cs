
using Application.Models;
using Domain.HelperClasses;
using MediatR;

namespace Application.Features.AuthFeature;

public class RefreshTokenCommand : IRequest<Response<JwtAuthResult>>
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
