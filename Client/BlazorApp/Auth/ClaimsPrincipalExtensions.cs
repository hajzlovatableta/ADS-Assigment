using System.Security.Claims;

namespace BlazorApp.Auth;

public static class ClaimsPrincipalExtensions
{
    // Returns the logged-in user's id, or null if no one is logged in
    public static int? GetUserId(this ClaimsPrincipal user)
    {
        if (user.Identity is null || !user.Identity.IsAuthenticated)
        {
            return null;
        }
        string? idAsString = user.Claims.SingleOrDefault(c => c.Type == "Id")?.Value;
        return int.TryParse(idAsString, out int id) ? id : null;
    }
}
