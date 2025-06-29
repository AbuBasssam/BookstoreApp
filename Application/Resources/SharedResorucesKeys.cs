namespace ApplicationLayer.Resources
{
    public class SharedResorucesKeys
    {
        #region General Response Status Keys
        public const string Success = "Success";
        public const string Created = "Created";
        public const string Deleted = "Deleted";
        public const string Failed = "Failed";
        #endregion

        #region HTTP Status Keys
        public const string BadRequest = "BadRequest";
        public const string Unauthorized = "Unauthorized";
        public const string NotFound = "NotFound";
        public const string UnprocessableEntity = "UnprocessableEntity";
        public const string InternalServerError = "InternalServerError";
        #endregion

        #region General Validation Keys
        public const string Required = "Required";
        public const string NotEmpty = "NotEmpty";
        public const string PropertyCannotBeNull = "PropertyCannotBeNull";
        public const string PropertyCannotBeEmpty = "PropertyCannotBeEmpty";
        #endregion

        #region User Input Validation Keys
        public const string EmailRequired = "EmailRequired";
        public const string PasswordRequired = "PasswordRequired";
        public const string TokenRequired = "TokenRequired";
        public const string CodeRequired = "CodeRequired";
        public const string NameRequired = "NameRequired";
        public const string GoogleIdTokenRequired = "GoogleIdTokenRequired";
        public const string RequestPayloadRequired = "RequestPayloadRequired";
        #endregion

        #region Format Validation Keys
        public const string InvalidEmail = "InvalidEmail";
        public const string InvalidCode = "InvalidCode";
        public const string NameLengthRange = "NameLengthRange";
        #endregion

        #region Authentication & Authorization Keys
        public const string InValidCredentials = "InValidCredentials";
        public const string InvalidAccessToken = "InvalidAccessToken";
        public const string InvalidGoogleToken = "InvalidGoogleToken";
        public const string ErrorConfirmingResetPasswordCode = "ErrorConfirmingResetPasswordCode";
        public const string ErrorResettingPassword = "ErrorResettingPassword";
        #endregion

        #region User Management Keys
        public const string EmailAlreadyExists = "EmailAlreadyExists";
        public const string UserNotFound = "UserNotFound";
        public const string UserCreationFailed = "UserCreationFailed";
        public const string EmailConfirmationFailed = "EmailConfirmationFailed";
        #endregion

        #region Token Validation Keys
        public const string ClaimsPrincipleIsNull = "ClaimsPrincipleIsNull";
        public const string InvalidUserId = "InvalidUserId";
        public const string InvalidEmailClaim = "InvalidEmailClaim";
        public const string NullRefreshToken = "NullRefreshToken";
        public const string RevokedRefreshToken = "RevokedRefreshToken";
        #endregion

        #region Token Extraction Keys
        public const string FailedExtractToken = "FailedExtractToken";
        public const string FailedExtractEmail = "FailedExtractEmail";
        public const string FailedToExtractCustomerId = "FailedToExtractCustomerId";
        #endregion

        #region Cooldown & Rate Limiting Keys
        public const string WaitCooldownPeriod = "WaitCooldownPeriod";
        public const string CooldownPeriodActive = "CooldownPeriodActive";
        public const string InvalidExpiredCode = "InvalidExpiredCode";
        #endregion

        #region Miscellaneous Keys
        public const string UnexpectedError = "UnexpectedError";
        #endregion

    }
}
