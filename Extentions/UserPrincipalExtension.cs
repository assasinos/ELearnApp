using System.Security.Claims;

namespace WebApplication3.Extentions;

public static class UserPrincipalExtension
{
    public static async Task<string> GetUserUID(this ClaimsPrincipal User)
    {
        return User.FindFirst("user_uid").Value;
    }
}