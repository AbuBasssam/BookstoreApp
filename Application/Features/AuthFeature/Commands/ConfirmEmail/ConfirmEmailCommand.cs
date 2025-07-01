using Application.Models;
using MediatR;

namespace Application.Features.AuthFeature;

public class ConfirmEmailCommand : IRequest<Response<bool>>
{
    public string VerificationToken { get; set; }
    public string ConfirmationCode { get; set; }


}
