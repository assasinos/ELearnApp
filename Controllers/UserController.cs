using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Claims;
using Dapper;
using ELearnApp.Extentions;
using ELearnApp.Models;
using ELearnApp.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace WebApplication3.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]

public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly MySqlConnection _mySqlConnection;

    public UserController(ILogger<UserController> logger,MySqlConnection connection)
    {
        _logger = logger;
        _mySqlConnection = connection;
    }


    [HttpGet]
    [ProducesResponseType(typeof(UserModel), 200)]
    [ProducesResponseType(typeof(UnauthorizedResult), 401)]
    [IsAuthenticated]
    public async Task<IActionResult> GetUser()
    {
        var user_uid = await User.GetUserUID();
        var user = await _mySqlConnection.QuerySingleOrDefaultAsync("SELECT `username`,`role`,`imgsrc` FROM `users` WHERE `user_uid` = @user_uid ", new{user_uid});

        
        
        
        return new JsonResult(new
        {
            user.username,
            role = user.role.ToString(),
            user.imgsrc
        });
    }
    


}

