using Application.Interfaces;
using Application.Models;
using MediatR;

namespace Application.Features.AuthFeature;

// Handler for the above command.
public class SendEmailConfirmationCodeHandler : IRequestHandler<SendEmailConfirmationCodeCommand, Response<string>>
{
    #region Field(s)

    private readonly IAuthService _authService;
    private readonly ResponseHandler _responseHandler;

    #endregion

    #region Constructor(s)
    public SendEmailConfirmationCodeHandler(IAuthService authService, ResponseHandler responseHandler)
    {
        _authService = authService;
        _responseHandler = responseHandler;
    }

    #endregion

    #region Handler(s)
    public async Task<Response<string>> Handle(SendEmailConfirmationCodeCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.SendEmailConfirmationCode(request.verificationToken);
        return _responseHandler.Success(result.data);
    }
    #endregion
}
