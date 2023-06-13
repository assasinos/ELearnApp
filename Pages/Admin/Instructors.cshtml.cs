using System.Collections;
using Dapper;
using ELearnApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using WebApplication3.Validators;

namespace ELearnApp.Pages.Admin;

[IsInRole(UserRole.Admin)]
public class Instructors : PageModel
{
    private readonly MySqlConnection _MySqlConnection;

    public Instructors(MySqlConnection mySqlConnection)
    {
        _MySqlConnection = mySqlConnection;
    }

    public async Task<IActionResult> OnGet()
    {
        InstructorModels = await GetInstructors();
        
        
        return Page();
    }

    private async Task<IEnumerable<UserModel>> GetInstructors()
    {
        return await  _MySqlConnection.QueryAsync<UserModel>("Select imgsrc, full_name, username,user_uid From users where role = 'Instructor'");
    }

    public IEnumerable<UserModel> InstructorModels { get; set; }
}