using System.Diagnostics;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using WebApplication3.Models;
using WebApplication3.Validators;

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
        var user_uid = User.FindFirst("user_uid").Value;
        var users = await _mySqlConnection.QueryAsync<UserModel>("SELECT `username`,`role`,`imgsrc` FROM `users` WHERE `user_uid` = @user_uid ", new{user_uid});

        var user = users.SingleOrDefault();
        
        
        return new JsonResult(new
        {
            user.username,
            role = user.role.ToString(),
            user.imgsrc
        });
    }


}

