using Application.Interfaces;
using Application.Models;
using MediatR;

namespace Application.Features.AuthFeature;

public class SendResetPasswordCommandHandler : IRequestHandler<SendResetPasswordCodeCommand, Response<string>>
{
    #region Field

    private readonly IAuthService _authService;
    private readonly ResponseHandler _responseHandler;

    #endregion

    #region Constructor
    public SendResetPasswordCommandHandler(IAuthService authService, ResponseHandler responseHandler)
    {
        _authService = authService;
        _responseHandler = responseHandler;

    }
    #endregion

    #region Handler
    public async Task<Response<string>> Handle(SendResetPasswordCodeCommand request, CancellationToken cancellationToken)
    {

        var response = await _authService.SendResetPasswordCode(request.Email);

        return _responseHandler.Success(response.data);



    }

    #endregion
}

