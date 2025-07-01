using Application.Models;
using Domain.Entities;
using Domain.HelperClasses;

namespace Application.Interfaces;
public interface IAuthService
{
    Task<JwtAuthResult> GetJwtAuthForuser(User User);
    Task<Result<string>> SignUp(User newUser, string password);
    Task<Result<string>> SendEmailConfirmationCode(string token);
    Task<Result<string>> ConfirmEmail(string verificationToken, string Code);

}
