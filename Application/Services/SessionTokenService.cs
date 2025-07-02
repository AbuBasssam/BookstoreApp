using Application.Interfaces;
using Domain.Entities;
using Domain.HelperClasses;
using Domain.Security;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services;

public class SessionTokenService : ISessionTokenService
{
    private readonly JwtSettings _jwtSetting;
    public SessionTokenService(JwtSettings jwtSetting)
    {
        _jwtSetting = jwtSetting;
    }
    public (JwtSecurityToken, string) GenerateVerificationToken(User user, int expiresInMinutes)
    {
        var claims = GetVerificationClaims(user);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var obj = new JwtSecurityToken(
            issuer: _jwtSetting.Issuer,
            audience: _jwtSetting.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: creds
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(obj);
        return (obj, accessToken);
    }

    public (JwtSecurityToken, string) GenerateResetToken(User user, int expiresInMinutes)
    {
        throw new NotImplementedException();
    }

    private List<Claim> GetVerificationClaims(User user)
    {
        return new List<Claim>
        {
            new Claim(SessionTokenClaims.IsVerificationToken, "true"),
            new Claim(nameof(UserClaimModel.Id), user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email !),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

        };
    }
}



