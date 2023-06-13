using System.ComponentModel.DataAnnotations;
using Dapper;
using ELearnApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using WebApplication3.Validators;

namespace ELearnApp.Pages.Profile;

[IsNotAuthenticated]
public class ConfirmAccount : PageModel
{

    private readonly MySqlConnection _mySqlConnection;
    
    public ConfirmAccount(MySqlConnection connection)
    {
        _mySqlConnection = connection;
    }
    
    
    public async Task<IActionResult> OnGet([Required]string token)
    {
        if (!await AccountConfirm(token))
        {
            
            //Token was not found
            return NotFound();
        }
        
        
        return Page();
    }

    private async Task<bool> AccountConfirm(string token)
    {
        var result = await _mySqlConnection.QuerySingleOrDefaultAsync<UserModel>("select * from `registrations` where `registration_uid` = @token", new {token});
        if (result is null)
        {
            return false;
        }

        await _mySqlConnection.ExecuteAsync("Delete from registrations where registration_uid = @token", new {token});

        await _mySqlConnection.ExecuteAsync("insert into users ( username, password, email, full_name) values (@username, @password, @email,@full_name)",result);
        return true;
    }
}