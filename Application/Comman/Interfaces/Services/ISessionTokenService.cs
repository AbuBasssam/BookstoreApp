using Domain.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace Application.Interfaces;

public interface ISessionTokenService
{
    (JwtSecurityToken, string) GenerateVerificationToken(User user, int expiresInMinutes);
    (JwtSecurityToken, string) GenerateResetToken(User user, int expiresInMinutes);
}