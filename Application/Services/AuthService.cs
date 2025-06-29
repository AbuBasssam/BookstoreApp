using Application.Interfaces;
using Domain.Entities;
using Domain.HelperClasses;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services;
public class AuthService : IAuthService
{
    #region Fields
    private readonly JwtSettings _jwtSettings;
    private readonly IUserService _userService;
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly SymmetricSecurityKey _signaturekey;
    private static string _SecurityAlgorithm = SecurityAlgorithms.HmacSha256Signature;
    #endregion

    #region Constructor(s)
    public AuthService(JwtSettings jwtSettings, IUserService userService, IRefreshTokenRepository refreshTokenRepo,
        UserManager<User> userManager, IUnitOfWork unitOfWork)
    {
        _jwtSettings = jwtSettings;
        _userService = userService;
        _refreshTokenRepo = refreshTokenRepo;
        _unitOfWork = unitOfWork;
        _signaturekey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret!));

    }
    #endregion

    #region Methods
    public async Task<JwtAuthResult> GetJwtAuthForuser(User User)
    {
        // 1) Generate jwtAccessTokon Object And String
        var (jwtAccessTokenObj, jwtAccessTokenString) = await _GenerateAccessToken(User);

        // 2) Generate RefreshToken Object
        var refreshTokenObj = _GenerateRefreshToken(User);

        // 3) Generate the JwtAuth for the user
        JwtAuthResult jwtAuthResult = _GetJwtAuthResult(jwtAccessTokenString, refreshTokenObj);

        // 4) Save AccessToken, RefreshToken In UserRefreshToken Table
        UserRefreshToken refreshTokenEntity = _GetUserRefreshToken(User, jwtAccessTokenObj, jwtAccessTokenString, refreshTokenObj);

        var result = await _refreshTokenRepo.AddAsync(refreshTokenEntity);

        await _unitOfWork.SaveChangesAsync();

        // 5) return the AuthResult for the user
        return jwtAuthResult;
    }
    private UserRefreshToken _GetUserRefreshToken(User User, JwtSecurityToken jwtAccessTokenObj, string jwtAccessTokenString, RefreshToken refreshTokenObj)
    {

        return new UserRefreshToken
        {
            UserId = User.Id,
            AccessToken = jwtAccessTokenString,
            RefreshToken = HashString(refreshTokenObj.Value),
            JwtId = jwtAccessTokenObj.Id,
            IsUsed = true,
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
            ExpiryDate = refreshTokenObj.ExpiresAt,
        };
    }
    private static JwtAuthResult _GetJwtAuthResult(string jwtAccessTokenString, RefreshToken refreshTokenObj)
    {


        return new JwtAuthResult
        {
            AccessToken = jwtAccessTokenString,
            RefreshToken = refreshTokenObj
        };
    }
    #endregion

    #region AccessToken Methods
    private List<Claim> _GenerateUserClaims(User User, List<string> Roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, User.UserName!),
            new Claim(ClaimTypes.Name, User.UserName !),
            new Claim(ClaimTypes.Email, User.Email !),
            new Claim(ClaimTypes.MobilePhone, User.PhoneNumber ??""),
            new Claim(nameof(UserClaimModel.Id), User.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

        };

        foreach (var role in Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }
    private JwtSecurityToken _GetJwtSecurityToken(List<Claim> claims)
    {
        return new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(_jwtSettings.AccessTokenExpireDate),
            signingCredentials: new SigningCredentials(_signaturekey, _SecurityAlgorithm)
        );
    }
    private async Task<(JwtSecurityToken, string)> _GenerateAccessToken(User User)
    {
        List<string> roles = await _userService.GetUserRolesAsync(User);

        var claims = _GenerateUserClaims(User, roles);

        JwtSecurityToken Obj = _GetJwtSecurityToken(claims);

        var Value = new JwtSecurityTokenHandler().WriteToken(Obj);

        return (Obj, Value);
    }

    #endregion

    #region RefreshToken Methods
    private RefreshToken _GenerateRefreshToken(User User)
    {
        return new RefreshToken()
        {
            Username = User.UserName!,
            Value = _GenerateRandomString64Length(),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpireDate)
        };
    }

    #endregion

    #region Helpers
    private string _GenerateRandomString64Length()
    {
        var randomNumber = new byte[32];
        using (var randomNumberGenerator = RandomNumberGenerator.Create())
        {
            randomNumberGenerator.GetBytes(randomNumber);
        }
        return Convert.ToBase64String(randomNumber);
    }
    private string HashString(string Value)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Value)).ToArray();
            return Convert.ToBase64String(hashedBytes);
        }
    }
    #endregion

}
