using Application.Models;
using MediatR;

namespace Application.Features.AuthFeature;
public class VerifyResetPasswordCodeCommand : IRequest<Response<string>>
{
    public string sessionToken { get; set; }
    public string ConfirmationCode { get; set; }

    public VerifyResetPasswordCodeCommand(string sessionToken, string confirmationCode)
    {
        this.sessionToken = sessionToken;
        this.ConfirmationCode = confirmationCode;
    }
}
