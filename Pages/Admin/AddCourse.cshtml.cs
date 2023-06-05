using System.Collections;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using WebApplication3.Models;
using WebApplication3.Validators;

namespace ELearnApp.Pages.Admin;

[IsInRole(UserRole.Admin)]
public class AddCourse : PageModel
{
    internal readonly MySqlConnection _mySqlConnection;

    public AddCourse(MySqlConnection mySqlConnection)
    {
        _mySqlConnection = mySqlConnection;
    }
    public async Task<IActionResult> OnGet()
    {

        Instructors = await GetAllInstructors();
        
        
        return Page();
    }

    private async Task<IEnumerable<UserModel>> GetAllInstructors()
    {
        return await _mySqlConnection.QueryAsync<UserModel>("SELECT full_name,user_uid FROM users WHERE role = 'instructor'");
    }

    internal IEnumerable<UserModel> Instructors { get; set; }
}