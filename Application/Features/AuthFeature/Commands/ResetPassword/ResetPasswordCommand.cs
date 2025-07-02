using Application.Models;
using MediatR;

namespace Application.Features.AuthFeature;
public class ResetPasswordCommand : IRequest<Response<bool>>
{
    public string SessionToken { get; set; }
    public string NewPassword { get; set; }

    public ResetPasswordCommand(string sessionToken, string newPassword)
    {
        NewPassword = newPassword;
        SessionToken = sessionToken;
    }
}
