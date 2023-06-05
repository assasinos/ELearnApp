using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApplication3.Models;

namespace WebApplication3.Validators;

public class IsInRoleAttribute : Attribute, IAuthorizationFilter
{
    private readonly UserRole _role;

    public IsInRoleAttribute(UserRole role)
    {
        _role = role;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Check if the user is authenticated
        if (!context.HttpContext.User.Identity.IsAuthenticated)
        {
            context.Result =  new RedirectToPageResult("/Error/401");
            return;
        }

        // Check if the user is in the required role
        if (!context.HttpContext.User.IsInRole(_role.ToString()))
        {
            context.Result = new RedirectToPageResult("/Error/403");
        }
    }
}