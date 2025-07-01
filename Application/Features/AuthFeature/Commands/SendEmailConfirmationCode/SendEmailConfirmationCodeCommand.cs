using Application.Models;
using MediatR;

namespace Application.Features.AuthFeature;

public record SendEmailConfirmationCodeCommand(string verificationToken) : IRequest<Response<string>>;

