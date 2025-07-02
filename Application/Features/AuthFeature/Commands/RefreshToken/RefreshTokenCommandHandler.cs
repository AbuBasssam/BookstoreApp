
using Application.Interfaces;
using Application.Models;
using ApplicationLayer.Resources;
using Domain.HelperClasses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;


namespace Application.Features.AuthFeature;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Response<JwtAuthResult>>
{
    #region Field(s)
    private readonly IAuthService _authenticationService;
    private readonly IUserService _userService;
    private readonly IStringLocalizer<SharedResources> _Localizer;
    private readonly ResponseHandler _responseHandler;
    #endregion

    #region Constructor
    public RefreshTokenCommandHandler(IAuthService authenticationService, IUserService userService, ResponseHandler responseHandler, IStringLocalizer<SharedResources> localizer)
    {
        _authenticationService = authenticationService;
        _userService = userService;
        _responseHandler = responseHandler;
        _Localizer = localizer;
    }
    #endregion

    #region Handler
    public async Task<Response<JwtAuthResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // 1) Get JwtSecurityToken Object
        var (jwtAccessTokenObj, jwtAccessTokenEx) = _authenticationService.GetJwtAccessTokenObjFromAccessTokenString(request.AccessToken);
        if (jwtAccessTokenEx != null) return _responseHandler.BadRequest<JwtAuthResult>(_Localizer[SharedResorucesKeys.InvalidAccessToken]);

        // 2) Get Claims Principle
        var (claimsPrinciple, claimsPrincipleEx) = _authenticationService.GetClaimsPrinciple(request.AccessToken);
        if (claimsPrincipleEx != null) return _responseHandler.BadRequest<JwtAuthResult>(_Localizer[SharedResorucesKeys.InvalidAccessToken]);

        // 3) Get UserId
        var (userId, userIdEx) = _authenticationService.GetUserIdFromJwtAccessTokenObj(jwtAccessTokenObj!);
        if (userIdEx != null) return _responseHandler.BadRequest<JwtAuthResult>(_Localizer[SharedResorucesKeys.InvalidAccessToken]);

        // 4) Validate RefreshToken
        var (refreshTokenObj, refreshTokenEx) = await _authenticationService.ValidateRefreshToken(userId, request.AccessToken, request.RefreshToken);

        // 5) Get User
        var user = await _userService.GetUserById(userId).FirstOrDefaultAsync();

        if (user == null)
            return _responseHandler.BadRequest<JwtAuthResult>(_Localizer[SharedResorucesKeys.InvalidAccessToken]);

        // 6) get new JwtAuth
        var jwtAuth = await _authenticationService.GetJwtAuthForuser(user);

        return _responseHandler.Success(jwtAuth);
    }
    #endregion
}
