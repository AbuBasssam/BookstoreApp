using Application.Interfaces;
using Application.Models;
using ApplicationLayer.Resources;
using Domain.AppMetaData;
using Domain.Entities;
using Domain.HelperClasses;
using Domain.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Serilog;
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
    private readonly IStringLocalizer<SharedResoruces> _Localizer;
    private readonly RoleManager<Role> _roleManager;
    private readonly VerificationTokenService _verificationTokenService;
    private static string _SecurityAlgorithm = SecurityAlgorithms.HmacSha256Signature;
    private static JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();


    #endregion

    #region Constructor(s)
    public AuthService(JwtSettings jwtSettings, IUserService userService, IRefreshTokenRepository refreshTokenRepo,
        UserManager<User> userManager, IUnitOfWork unitOfWork, IStringLocalizer<SharedResoruces> localizer, RoleManager<Role> roleManager)
    {
        _jwtSettings = jwtSettings;
        _userService = userService;
        _refreshTokenRepo = refreshTokenRepo;
        _unitOfWork = unitOfWork;
        _Localizer = localizer;
        _roleManager = roleManager;
        _signaturekey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret!));
        _verificationTokenService = new VerificationTokenService(jwtSettings);
    }
    #endregion

    #region Methods
    public async Task<JwtAuthResult> GetJwtAuthForuser(User User)
    {
        // 1) Generate jwtAccessTokon Object And String
        var (jwtAccessTokenObj, jwtAccessTokenString) = _GenerateAccessToken(User);

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
    public bool IsValidAccessToken(string AccessTokenStr)
    {
        try
        {
            var (jwtAccessTokenObj, jwtAccesTokenEx) = GetJwtAccessTokenObjFromAccessTokenString(AccessTokenStr);
            if (jwtAccesTokenEx != null) return false;

            GetClaimsPrinciple(AccessTokenStr);

            if (jwtAccessTokenObj!.ValidTo < DateTime.UtcNow) return false;


            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);

            return false;
        }
    }
    public async Task<Result<string>> SignUp(User newUser, string password)
    {
        try
        {
            // Step 1: Check if the user already exists
            var userExistsResult = await _CheckIfUserExists(newUser.Email);
            if (!userExistsResult.IsSuccess) return userExistsResult;

            // Step 2: Create a new user
            var createUserResult = await _CreateUser(newUser, password);
            if (!createUserResult.IsSuccess) return createUserResult;

            // Step 3: Generate verification token for Confirmation operation
            string sessionToken = _verificationTokenService.GenerateVerificationToken(newUser, 15);


            return Result<string>.Success(sessionToken);


        }
        catch (Exception ex)
        {
            Log.Error(ex, messageTemplate: ex.Message);

            return Result<string>.Failure([]);

        }
    }

    #endregion

    #region AccessToken Methods
    private List<Claim> _GenerateUserClaims(User User, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, User.UserName!),
            new Claim(ClaimTypes.Name, User.UserName !),
            new Claim(ClaimTypes.Email, User.Email !),
            new Claim(nameof(UserClaimModel.Id), User.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

        };

        claims.Add(new Claim(ClaimTypes.Role, role));


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
    public (JwtSecurityToken?, Exception?) GetJwtAccessTokenObjFromAccessTokenString(string AccessToken)
    {
        try
        {
            return ((JwtSecurityToken)_tokenHandler.ReadToken(AccessToken), null);
        }
        catch (Exception ex)
        {
            return (null, ex);
        }
    }
    public (ClaimsPrincipal?, Exception?) GetClaimsPrinciple(string AccessToken)
    {
        var parameters = _GetTokenValidationParameters();

        try
        {
            var principal = _tokenHandler.ValidateToken(AccessToken, parameters, out SecurityToken validationToken);

            if (validationToken is JwtSecurityToken jwtToken && jwtToken.Header.Alg.Equals(_SecurityAlgorithm))
                return (principal, null);

            return (null, new ArgumentNullException(_Localizer[SharedResorucesKeys.ClaimsPrincipleIsNull]));
        }
        catch (Exception ex)
        {
            return (null, ex);
        }
    }
    private TokenValidationParameters _GetTokenValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = _jwtSettings.ValidateIssuer,
            ValidIssuers = new[] { _jwtSettings.Issuer },

            ValidateAudience = _jwtSettings.ValidateAudience,
            ValidAudience = _jwtSettings.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret!)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }
    private (JwtSecurityToken, string) _GenerateAccessToken(User User)
    {

        string role = User.Role?.Name;
        if (string.IsNullOrEmpty(role))
        {
            throw new InvalidOperationException(_Localizer[SharedResorucesKeys.RoleNotAssigned]);
        }
        var claims = _GenerateUserClaims(User, role);

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

    #region Sign In methods

    private static JwtAuthResult _GetJwtAuthResult(string jwtAccessTokenString, RefreshToken refreshTokenObj)
    {


        return new JwtAuthResult
        {
            AccessToken = jwtAccessTokenString,
            RefreshToken = refreshTokenObj
        };
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

    #endregion

    #region Sign Up methods
    private async Task<Result<string>> _CheckIfUserExists(string email)
    {
        var existingUser = await _userService.GetUserByEmailAsync(email).FirstOrDefaultAsync();
        return existingUser == null
            ? Result<string>.Success(string.Empty)
            : Result<string>.Failure(new List<string> { _Localizer[SharedResorucesKeys.EmailAlreadyExists] });
    }

    private async Task<Result<string>> _CreateUser(User user, string password)
    {
        user.RoleID = _roleManager.Roles.FirstOrDefault(r => r.Name.Equals(Roles.Member))?.Id ?? throw new InvalidOperationException("Role not found");
        var result = await _userService.CreateUserAsync(user, password);

        if (result.Succeeded) return Result<string>.Success(string.Empty);

        var errors = result.Errors.Select(e => e.Description).ToList();
        return Result<string>.Failure(errors);
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
