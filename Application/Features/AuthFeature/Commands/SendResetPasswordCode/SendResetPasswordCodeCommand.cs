using Application.Models;
using MediatR;

namespace Application.Features.AuthFeature;
public class SendResetPasswordCodeCommand : IRequest<Response<string>>
{
    public string Email { get; set; }

    public SendResetPasswordCodeCommand(string email)
    {
        Email = email;
    }

}
