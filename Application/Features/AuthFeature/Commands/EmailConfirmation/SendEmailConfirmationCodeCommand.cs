using Application.Models;
using MediatR;

namespace Application.Features.AuthFeature;
// Command for sending verificationToken confirmation OTP.

public record SendEmailConfirmationCodeCommand(string verificationToken) : IRequest<Response<string>>;

