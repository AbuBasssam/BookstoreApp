using Application.Models;
using Domain.Entities;
using Domain.HelperClasses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.Interfaces;
public interface IAuthService
{
    Task<JwtAuthResult> GetJwtAuthForuser(User User);
    Task<Result<string>> SignUp(User newUser, string password);
    Task<Result<string>> SendEmailConfirmationCode(string token);
    Task<Result<string>> ConfirmEmail(string verificationToken, string Code);
    Task<Result<string>> SendResetPasswordCode(string email);
    Task<Result<string>> ConfirmResetPasswordCode(string sessionToken, string Code);
    Task<bool> ResetPasassword(string sessionToken, string newPassword);
    (JwtSecurityToken?, Exception?) GetJwtAccessTokenObjFromAccessTokenString(string AccessToken);
    (int, Exception?) GetUserIdFromJwtAccessTokenObj(JwtSecurityToken jwtAccessTokenObj);
    (ClaimsPrincipal?, Exception?) GetClaimsPrinciple(string AccessToken);
    bool IsValidAccessToken(string AccessTokenStr);
    Task<(UserRefreshToken?, Exception?)> ValidateRefreshToken(int UserId, string AccessTokenStr, string RefreshTokenStr);





}
