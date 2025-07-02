using Application.Interfaces;
using Application.Models;
using MediatR;

namespace Application.Features.AuthFeature;

public class AuthorizeUserQueryHandler : IRequestHandler<AuthorizeUserQuery, Response<string>>
{
    #region Field(s)

    private readonly IAuthService _authService;
    private readonly ResponseHandler _responseHandler1;

    #endregion

    #region Constructor(s)
    public AuthorizeUserQueryHandler(IAuthService authService, ResponseHandler responseHandler)
    {
        _authService = authService;
        _responseHandler1 = responseHandler;
    }
    #endregion

    #region Handler(s)

    public async Task<Response<string>> Handle(AuthorizeUserQuery request, CancellationToken cancellationToken)
    {
        return (_authService.IsValidAccessToken(request.AccessToken)) ? _responseHandler1.Success<string>("Valid") : _responseHandler1.BadRequest<string>("Not valid");

    }

    #endregion
}
