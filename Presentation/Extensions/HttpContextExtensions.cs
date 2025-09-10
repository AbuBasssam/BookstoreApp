using Microsoft.AspNetCore.Http;

namespace Presentation.Extensions;
public static class HttpContextExtensions
{
    public static string? GetAuthToken(this HttpContext context)
    {
        return context.Request.Headers["Authorization"]
            .FirstOrDefault()?
            .Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase)
            .Trim();
    }

    public static string? GetClientIP(this HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString();
    }
    //get language from request header
    public static string GetRequestLanguage(this HttpContext context)
    {
        var lang = context.Request.Headers["Accept-Language"].FirstOrDefault();
        if (string.IsNullOrEmpty(lang))
            return "en"; // default language
        return lang.Split(',').FirstOrDefault()?.Trim().ToLower() ?? "en";
    }

}
