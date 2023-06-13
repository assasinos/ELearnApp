
using Dapper;
using ELearnApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace ELearnApp.Pages.Instructors;

public class Index : PageModel
{
    internal IEnumerable<UserModel> Instructors;
    private MySqlConnection _mySqlConnection;
    private readonly ILogger<Index> _logger;

    public Index(ILogger<Index> logger, MySqlConnection connection)
    {
        _logger = logger;
        _mySqlConnection = connection;
    }

    public async Task<IActionResult> OnGet()
    {
        Instructors = await GetInstructors();
        
        return Page();
    }


    private async Task<IEnumerable<UserModel>> GetInstructors()
    {
        return await _mySqlConnection.QueryAsync<UserModel>(
            "SELECT `full_name`, `imgsrc`,`bio` from `users` WHERE `role` = 'Instructor'");
    }
}