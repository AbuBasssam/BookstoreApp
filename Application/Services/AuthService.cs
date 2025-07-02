using Application.Interfaces;
using Application.Models;
using ApplicationLayer.Resources;
using Domain.AppMetaData;
using Domain.Entities;
using Domain.Enums;
using Domain.HelperClasses;
using Domain.Security;
using Interfaces;
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
    private readonly IEmailsService _emailsService;
    private readonly ISessionTokenService _sessionTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly IOtpRepsitory _otpRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly SymmetricSecurityKey _signaturekey;
    private readonly IStringLocalizer<SharedResources> _Localizer;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private static string _SecurityAlgorithm = SecurityAlgorithms.HmacSha256Signature;
    private static JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();


    #endregion

    #region Constructor(s)
    public AuthService(JwtSettings jwtSettings, IUserService userService, ISessionTokenService sessionTokenService, IEmailsService emailsService, IRefreshTokenRepository refreshTokenRepo, IOtpRepsitory otpRepo,
        UserManager<User> userManager, IUnitOfWork unitOfWork, IStringLocalizer<SharedResources> localizer, RoleManager<Role> roleManager)
    {
        _jwtSettings = jwtSettings;
        _userService = userService;
        _emailsService = emailsService;
        _refreshTokenRepo = refreshTokenRepo;
        _otpRepo = otpRepo;
        _unitOfWork = unitOfWork;
        _Localizer = localizer;
        _userManager = userManager;
        _roleManager = roleManager;
        _signaturekey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret!));
        _sessionTokenService = sessionTokenService;
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
            UserRefreshToken verificationToken = _GenerateVerificationToken(newUser, 15);

            //Step 4:Save verification token in database
            await _refreshTokenRepo.AddAsync(verificationToken);

            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Success(verificationToken.AccessToken!);


        }
        catch (Exception ex)
        {
            Log.Error(ex, messageTemplate: ex.Message);

            return Result<string>.Failure([]);

        }
    }

    public async Task<Result<string>> SendEmailConfirmationCode(string token)
    {
        try
        {
            // Step 1: Validate the session token
            var tokenValidationResult = await _ValidateSessionToken(token);

            if (!tokenValidationResult.IsSuccess) return tokenValidationResult;

            // Step 2: Extract email from the token
            var getEmailResult = _GetEmailFromSessionToken(token);

            if (!getEmailResult.IsSuccess) return getEmailResult;

            string email = getEmailResult.data!;
            // End new Area

            // Step 1: Retrieve the user by email
            var validateResult = await _ValidateUserExists(email);

            // For security, return a generic success message if user not found.
            if (!validateResult.IsSuccess) return Result<string>.Success(_Localizer[SharedResorucesKeys.Success]);

            User user = validateResult.data!;

            // Step 2: Handle OTP expiration and cooldown
            var otpValidationResult = await _ValidateOtpLifecycleAsync(email, user.Id, 2, enOtpType.ConfirmEmail);

            if (!otpValidationResult.IsSuccess) return otpValidationResult;

            // Step 3: Generate OTP and message
            string otpCode = _GenerateOTPCode();

            string message = $"Your email confirmation code is: {otpCode}";

            // Step 4: Send email
            await SendOtpEmail(user.Email!, otpCode, "Confirm Your Email");

            //Step 5: _SaveOtp in database
            int minutesValidDuration = 5;
            await _SaveOtpToDb(user.Id, otpCode, enOtpType.ConfirmEmail, minutesValidDuration);

            // Step 6: Commit changes to the database
            await _unitOfWork.SaveChangesAsync();


            return Result<string>.Success(_Localizer[SharedResorucesKeys.Success]);



        }
        catch (Exception ex)
        {
            Log.Error(ex, messageTemplate: ex.Message);
            return Result<string>.Success(string.Empty);

        }
    }

    public async Task<Result<string>> ConfirmEmail(string verificationToken, string Code)
    {
        try
        {
            // Step 1: Validate the session token
            var tokenValidationResult = await _ValidateSessionToken(verificationToken);
            if (!tokenValidationResult.IsSuccess) return tokenValidationResult;

            // Step 2: Extract email from the token
            var getEmailResult = _GetEmailFromSessionToken(verificationToken);

            if (!getEmailResult.IsSuccess) return getEmailResult;

            string email = getEmailResult.data!;

            // Step 3: Retrieve the user

            var validateResult = await _ValidateUserExists(email);

            if (!validateResult.IsSuccess) return Result<string>.Failure([_Localizer[SharedResorucesKeys.UserNotFound]]);

            User user = validateResult.data!;

            // Step 4: Get last confirmation code
            Otp? otp = await _GetLastCode(email, enOtpType.ConfirmEmail);
            if (otp == null) return Result<string>.Failure([_Localizer[SharedResorucesKeys.InvalidExpiredCode]]);

            // Step 5: Validate the OTP.
            var otpValidationResult = _ValidateOtp(Code, otp);

            if (!otpValidationResult.IsSuccess) return otpValidationResult;

            // Step 6: Confirm email
            await _ConfirmUserEmail(user);

            //Step 7: Force expire the OTP.

            await _ExpireOTP(otp);

            // Step 8: Deactivate the verification token
            await _DeactiveVerificationToken(user.Id);

            // Step 9: Commit changes to the database
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Success(_Localizer[SharedResorucesKeys.Success]);

        }
        catch (Exception ex)
        {
            Log.Error(ex, messageTemplate: ex.Message);
            return Result<string>.Failure([_Localizer[SharedResorucesKeys.InvalidExpiredCode]]);
        }
    }

    public async Task<Result<string>> SendResetPasswordCode(string email)
    {
        try
        {
            // Step 1: Validate that the user exists.
            var validateResult = await _ValidateUserExists(email);

            // For security, return a generic success message if user not found.
            if (!validateResult.IsSuccess) return Result<string>.Success(_Localizer[SharedResorucesKeys.Success]);

            User user = validateResult.data;

            // Step 2: Validate and expire any old OTP for reset password.
            var otpValidationResult = await _ValidateOtpLifecycleAsync(email, user.Id, 1, enOtpType.ResetPassword);

            if (!otpValidationResult.IsSuccess) return Result<string>.Failure(otpValidationResult.Errors);

            // Step 3: Generate OTP and message
            string otpCode = _GenerateOTPCode();

            string message = $"Your reset password code is: {otpCode}";

            // Step 4: Send OTP code via email.
            await SendOtpEmail(user.Email!, otpCode, "Reset Password");

            // Step 5: Generate session token 
            UserRefreshToken sessionToken = _GenerateResetPasswordToken(user, 3);

            // Step 6: Save OTP & reset password token in the database.
            int minutesValidDuration = 3;
            await _SaveOtpToDb(user.Id, otpCode, enOtpType.ResetPassword, minutesValidDuration);

            await _refreshTokenRepo.AddAsync(sessionToken);

            // Step 7: Commit all changes.
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Success(sessionToken.AccessToken!);






        }
        catch (Exception ex)
        {
            Log.Error(ex, messageTemplate: ex.Message);

            return Result<string>.Success(string.Empty);
        }
    }

    public async Task<Result<string>> ConfirmResetPasswordCode(string sessionToken, string code)
    {
        try
        {
            // Step 1: Validate the session token.
            var tokenValidationResult = await _ValidateSessionToken(sessionToken);

            if (!tokenValidationResult.IsSuccess) return tokenValidationResult;

            // Step 2: Extract email from the token
            var getEmailResult = _GetEmailFromSessionToken(sessionToken);

            if (!getEmailResult.IsSuccess) return getEmailResult;

            string email = getEmailResult.data!;

            // Step 3: Retrieve the user
            var validateResult = await _ValidateUserExists(email);

            if (!validateResult.IsSuccess) return Result<string>.Failure([_Localizer[SharedResorucesKeys.UserNotFound]]);

            User user = validateResult.data!;

            // Step 4: Get last confirmation code
            Otp? otp = await _GetLastCode(email, enOtpType.ResetPassword);

            if (otp == null) return Result<string>.Failure([_Localizer[SharedResorucesKeys.InvalidExpiredCode]]);

            // Step 5: Validate the OTP.
            var otpValidationResult = _ValidateOtp(code, otp);

            if (!otpValidationResult.IsSuccess) return otpValidationResult;

            // Step 6: Expire the OTP and the active session token.

            await _ExpireOTP(otp);

            await _DeactiveVerificationToken(user.Id);

            // Step 7: Generate a new session token for reset password.
            var newSessionToken = _GenerateNewResetToken(user);

            // Step 8: Save the new session token and commit all changes.
            await _refreshTokenRepo.AddAsync(newSessionToken);

            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Success(newSessionToken.AccessToken!);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error confirming reset password code: {Message}", ex.Message);
            return Result<string>.Failure(new List<string> { _Localizer[SharedResorucesKeys.ErrorConfirmingResetPasswordCode] });
        }




    }

    public async Task<bool> ResetPasassword(string sessionToken, string newPassword)
    {
        try
        {

            // Step 1: Validate the session token.
            var tokenValidationResult = await _ValidateSessionToken(sessionToken);

            if (!tokenValidationResult.IsSuccess) return false;

            // Step 2: Extract email from the token
            var getEmailResult = _GetEmailFromSessionToken(sessionToken);

            if (!getEmailResult.IsSuccess) return false;

            string email = getEmailResult.data!;
            // Step 3: Retrieve the user

            var validateResult = await _ValidateUserExists(email);

            if (!validateResult.IsSuccess) return false;

            User user = validateResult.data!;

            // Step 5: Remove the user's old password.
            if (!await RemoveUserPassword(user)) return false;

            // Step 6: Set the new password.
            if (!await AddUserPassword(user, newPassword)) return false;

            // Step 7: Deactivate the session token.
            await _DeactiveVerificationToken(user.Id);


            // Step 8: Commit the changes.
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error resetting password for token: {SessionToken}", sessionToken);
            return false;
        }


    }
    private async Task<bool> RemoveUserPassword(User user)
    {
        // Removes the user's old password.
        var result = await _userManager.RemovePasswordAsync(user);
        return result.Succeeded;
    }
    private async Task<bool> AddUserPassword(User user, string newPassword)
    {
        // Sets the user's new password.
        var result = await _userManager.AddPasswordAsync(user, newPassword);
        return result.Succeeded;
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
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpireDate),
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

    public (string, Exception?) GetUserEmailFromJwtAccessTokenObj(JwtSecurityToken jwtAccessTokenObj)
    {
        var emailClaim = jwtAccessTokenObj.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        return string.IsNullOrEmpty(emailClaim) ?
            ("", new ArgumentNullException(_Localizer[SharedResorucesKeys.InvalidEmailClaim])) : (emailClaim, null);
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

    #region verify Email methods
    private async Task<Result<string>> _ValidateSessionToken(string sessionToken)
    {

        bool isExpired = await _refreshTokenRepo.IsTokenExpired(sessionToken);


        return isExpired ? Result<string>.Failure([_Localizer[SharedResorucesKeys.InvalidExpiredCode]]) : Result<string>.Success("");//"Invalid/Expired Code"

    }

    private Result<string> _GetEmailFromSessionToken(string sessionToken)
    {
        (JwtSecurityToken? jwtAccessTokenObj, Exception? exception) = GetJwtAccessTokenObjFromAccessTokenString(sessionToken);

        (string email, Exception? emailException) = GetUserEmailFromJwtAccessTokenObj(jwtAccessTokenObj);
        if (string.IsNullOrEmpty(email)) return Result<string>.Failure([_Localizer[SharedResorucesKeys.FailedExtractEmail]]);

        return Result<string>.Success(email);
    }

    private async Task SendOtpEmail(string email, string otpCode, string subject)
    {
        string message = $"Your code to reset password: {otpCode}";
        await _emailsService.SendEmail(email, message, subject);
    }

    private async Task _SaveOtpToDb(int UserID, string otp, enOtpType otpType, int minutesValidDuration)
    {
        var otpEntity = new Otp
        {
            Code = HashString(otp),
            Type = otpType,
            CreationTime = DateTime.UtcNow,
            ExpirationTime = DateTime.UtcNow.AddMinutes(minutesValidDuration),
            UserID = UserID
        };
        await _otpRepo.AddAsync(otpEntity);


    }

    private async Task<Result<string>> _ValidateOtpLifecycleAsync(string email, int userId, int minutesCooldownPeriod, enOtpType otpType)
    {
        var oldOtp = await _GetLastCode(email, otpType);

        if (oldOtp == null) return Result<string>.Success(string.Empty);


        var timeSinceLastOtp = DateTime.UtcNow - oldOtp.CreationTime;
        var cooldownPeriod = TimeSpan.FromMinutes(minutesCooldownPeriod);

        if (timeSinceLastOtp < cooldownPeriod)
        {
            return Result<string>.Failure(new List<string>
            {
                _Localizer[SharedResorucesKeys.CooldownPeriodActive]

            });
        }

        if (!IsCodeExpired(oldOtp.ExpirationTime))
        {
            await _ExpireOTP(oldOtp);

            // Expire the session token.
            await _DeactiveVerificationToken(userId);

        }

        return Result<string>.Success(string.Empty);
    }

    private Result<string> _ValidateOtp(string Code, Otp otp)
    {

        string confirmResult = IsCodeWrongOrExpired(Code, otp);
        if (confirmResult == "Expired") return Result<string>.Failure([_Localizer[SharedResorucesKeys.InvalidExpiredCode]]);

        return Result<string>.Success("");
    }

    private async Task _ConfirmUserEmail(User user)
    {
        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);
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

    private string _GenerateOTPCode()
    {
        Random generator = new Random();
        return generator.Next(100000, 1000000).ToString("D6");
    }

    private async Task _DeactiveVerificationToken(int UserID)
    {
        var activeRefreshToken =
               await _refreshTokenRepo
               .GetActiveSessionTokenByUserId(UserID)
               .FirstOrDefaultAsync();

        if (activeRefreshToken == null) return;

        activeRefreshToken.IsRevoked = true;

        activeRefreshToken.IsUsed = false;

        activeRefreshToken.ExpiryDate = DateTime.UtcNow;

        await _refreshTokenRepo.UpdateAsync(activeRefreshToken);

    }

    private UserRefreshToken _GenerateVerificationToken(User user, int minutesValidDuration)
    {
        var (jwtAccessTokenObj, jwtAccessTokenString) = _sessionTokenService.GenerateVerificationToken(user, minutesValidDuration);


        return new UserRefreshToken
        {
            UserId = user.Id,
            AccessToken = jwtAccessTokenString,
            RefreshToken = null,
            JwtId = jwtAccessTokenObj.Id,
            IsUsed = true,
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddMinutes(minutesValidDuration)
        };



    }

    private UserRefreshToken _GenerateResetPasswordToken(User user, int minutesValidDuration)
    {
        var (jwtAccessTokenObj, jwtAccessTokenString) = _sessionTokenService.GenerateResetToken(user, minutesValidDuration);


        return new UserRefreshToken
        {
            UserId = user.Id,
            AccessToken = jwtAccessTokenString,
            RefreshToken = null,
            JwtId = jwtAccessTokenObj.Id,
            IsUsed = true,
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddMinutes(minutesValidDuration)
        };



    }

    private async Task<Result<User>> _ValidateUserExists(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        return user == null ?
            Result<User>.Failure(new List<string> { _Localizer[SharedResorucesKeys.UserNotFound] }) :

            Result<User>.Success(user);
    }

    private async Task _ExpireOTP(Otp otp)
    {
        otp.ExpirationTime = DateTime.UtcNow;
        await _otpRepo.UpdateAsync(otp);
    }

    private UserRefreshToken _GenerateNewResetToken(User user)
    {
        return _GenerateResetPasswordToken(user, 3);
    }


    #endregion

    #region Comfirmation Helpers

    private string IsCodeWrongOrExpired(string otp, Otp entity)
    {
        return (IsCodeWrong(otp, entity.Code) || IsCodeExpired(entity.ExpirationTime)) ? "Expired" : "Valid";

    }

    private bool IsCodeWrong(string otpCode, string savedCode)
    {
        string hashedOtp = HashString(otpCode);

        return savedCode != hashedOtp;
    }

    private bool IsCodeExpired(DateTime savedCodeExpirationTime)
        => !(savedCodeExpirationTime > DateTime.UtcNow);

    private async Task<Otp?> _GetLastCode(string Email, enOtpType otpType)
    {
        return await _otpRepo
            .GetLastOtpAsync(Email, otpType)
            .FirstOrDefaultAsync();
    }

    #endregion






}
