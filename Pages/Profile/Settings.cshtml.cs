﻿using Dapper;
using ELearnApp.Models;
using ELearnApp.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace ELearnApp.Pages.Profile;


[IsAuthenticated]
public class Settings : PageModel
{

    private readonly MySqlConnection _mySqlConnection;

    public Settings(MySqlConnection connection)
    {
        _mySqlConnection = connection;
    }

    internal UserModel UserInformation = null!;
    
    
    public async Task<IActionResult> OnGet()
    {
        var user_uid = User.FindFirst("user_uid")?.Value;
        if (user_uid is null) return RedirectToPage("/Index");
        UserInformation = await GetUserInfo(user_uid);
        
        
        return  Page();
    }

    private async Task<UserModel> GetUserInfo(string userUid)
    {
        return await _mySqlConnection.QuerySingleOrDefaultAsync<UserModel>("Select full_name,email from users where user_uid = @userUid", new {userUid});
    }
}