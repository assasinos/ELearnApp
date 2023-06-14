using ELearnApp.Models;
using ELearnApp.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ELearnApp.Pages.Instructor;

[IsInRole(UserRole.Instructor)]
public class AddCourse : PageModel
{
    public async Task<IActionResult> OnGet()
    {
        return Page();
    }
}