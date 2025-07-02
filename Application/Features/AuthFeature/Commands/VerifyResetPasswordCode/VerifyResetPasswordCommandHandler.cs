using Application.Interfaces;
using Application.Models;
using MediatR;

namespace Application.Features.AuthFeature;

public class VerifyResetPasswordCommandHandler : IRequestHandler<VerifyResetPasswordCodeCommand, Response<string>>
{
    #region Field(s)

    private readonly IAuthService _authService;
    private readonly ResponseHandler _responseHandler;

    #endregion

    #region Constructor(s)
    public VerifyResetPasswordCommandHandler(IAuthService authService, ResponseHandler responseHandler)
    {
        _authService = authService;
        _responseHandler = responseHandler;

    }
    #endregion

    #region Handler(s)
    public async Task<Response<string>> Handle(VerifyResetPasswordCodeCommand request, CancellationToken cancellationToken)
    {
        var resutl = await _authService.ConfirmResetPasswordCode(request.sessionToken, request.ConfirmationCode);

        return resutl.IsSuccess ?
            _responseHandler.Success(resutl.data) :
            _responseHandler.BadRequest<string>(string.Join('\n', resutl.Errors));
    }
    #endregion
}
