using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using WebApplication3.Models;

namespace ELearnApp.Pages.Admin;

public class Instructor : PageModel
{

    private readonly MySqlConnection _mySqlConnection;
    public Instructor(MySqlConnection mySqlConnection)
    {
        _mySqlConnection = mySqlConnection;
    }


    internal UserModel? InstructorModel;
    
    public async Task<IActionResult> OnGet(string userUid)
    {

        InstructorModel = await GetInstructorInfo(userUid);

        if (InstructorModel is null)
        {
            return NotFound();
        }
        
        return Page();
    }

    private async Task<UserModel?> GetInstructorInfo(string userUid)
    {
        return await _mySqlConnection.QuerySingleOrDefaultAsync<UserModel>(
            "SELECT user_uid,email,bio,imgsrc,full_name,username,created_at FROM users WHERE user_uid = @userUid", new { userUid });
    }
}