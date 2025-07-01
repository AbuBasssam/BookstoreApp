using Application.Interfaces;
using Application.Models;
using MediatR;

namespace Application.Features.AuthFeature;

public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, Response<bool>>
{
    #region Field(s)

    private readonly IAuthService _authService;
    private readonly ResponseHandler _responseHandler;

    #endregion


    #region Constructor(s)

    public ConfirmEmailHandler(IAuthService authService, ResponseHandler responseHandler)
    {
        _authService = authService;
        _responseHandler = responseHandler;
    }

    #endregion


    #region Handler(s)
    public async Task<Response<bool>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var confirmationResult = await _authService.ConfirmEmail(request.VerificationToken, request.ConfirmationCode);

        return confirmationResult.IsSuccess ?
            _responseHandler.Success(true) :
            _responseHandler.BadRequest<bool>(string.Join(',', confirmationResult.Errors));
    }
    #endregion
}

