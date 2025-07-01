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
}
