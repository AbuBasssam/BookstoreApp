using Application.Interfaces;
using Application.Models;
using MediatR;

namespace Application.Features.AuthFeature;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Response<bool>>
{
    #region Field(s)

    private readonly IAuthService _authService;
    private readonly ResponseHandler _responseHandler;

    #endregion

    #region Constructor(s)
    public ResetPasswordCommandHandler(IAuthService authService, ResponseHandler responseHandler)
    {
        _authService = authService;
        _responseHandler = responseHandler;

    }
    #endregion

    #region Handler(s)

    public async Task<Response<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var resutl = await _authService.ResetPasassword(request.SessionToken, request.NewPassword);

        return resutl ?
            _responseHandler.Success(true) :
            _responseHandler.BadRequest<bool>();
    }

    #endregion
}
