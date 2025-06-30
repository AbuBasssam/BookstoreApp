using Domain.Entities;
using Domain.HelperClasses;
using Domain.Security;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services;

public class VerificationTokenService
{
    private readonly JwtSettings _jwtSetting;
    public VerificationTokenService(JwtSettings jwtSetting)
    {
        _jwtSetting = jwtSetting;
    }
    public string GenerateVerificationToken(User user, int expiresInMinutes)
    {
        var claims = GetVerificationClaims(user);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSetting.Issuer,
            audience: _jwtSetting.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    private List<Claim> GetVerificationClaims(User user)
    {
        return new List<Claim>
        {
            new Claim(VerificationClaims.IsVerificationToken, "true"),
            new Claim(nameof(UserClaimModel.Id), user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email !),


        };
    }
}



//// Program.cs Configuration
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("VerificationOnly", policy =>
//        policy.Requirements.Add(new VerificationOnlyRequirement()));
//});

//builder.Services.AddScoped<IAuthorizationHandler, VerificationOnlyHandler>();
//1. POST /signup 
//   → Returns: { success: true, verificationToken: "..." }

//2.POST / send - email - confirmation - otp
//   → Header: Authorization: Bearer[verificationToken]

//3.POST / verify - email - confirmation - otp
//   → Header: Authorization: Bearer[verificationToken]
//   → Returns: Updated verificationToken with emailVerified = true

//4. POST /confirm-phone-number
//   → Header: Authorization: Bearer[verificationToken]

//5.POST / verify - phone - number
//   → Header: Authorization: Bearer[verificationToken]
//   → Returns: Full JWT token for app access