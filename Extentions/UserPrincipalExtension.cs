using System.Security.Claims;

namespace ELearnApp.Extentions;

public static class UserPrincipalExtension
{
    public static async Task<string> GetUserUID(this ClaimsPrincipal User)
    {
        return User.FindFirst("user_uid").Value;
    }
}