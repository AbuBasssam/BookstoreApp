namespace Domain.AppMetaData;
public static class Router
{
    #region  Root Constants
    private const string _root = "api";
    private const string _version = "v1";

    // api/v1
    private const string _rule = _root + "/" + _version;

    // sub routes
    // api/v1/Student/{Id}
    private const string _ById = "/{Id}";

    // api/v1/<Controller>/query? key=value & key=value
    private const string _Query = "/query";

    #endregion

    #region Authentication Routes
    public class AuthenticationRouter
    {
        public const string BASE = _rule + "/authentication";
        public const string SignIn = BASE + "/signin";
        public const string SignUp = BASE + "/signup";

        public const string VerifyEmailCode = BASE + "/verify-email-confirmation-otp";

        public const string ConfirmEmailCode = BASE + "/send-email-confirmation-otp";


        public const string ResetPasswordCode = BASE + "/reset-password-code";

        public const string VerifyResetPasswordCode = BASE + "/verify-reset-password-code";

        public const string ResetPassword = BASE + "/reset-password";


        public const string ValidateRefreshToken = BASE + "/validate-refresh-token/{token}";

        public const string RefreshToken = BASE + "/refresh-token";


        public const string Logout = BASE + "/logout";

    }

    #endregion
    public class TestRouter()
    {
        public const string BASE = _rule + "/test";
        public const string Global = BASE + "/global";
        public const string Sensitive = "/" + BASE + "/sensitive";
    }


}