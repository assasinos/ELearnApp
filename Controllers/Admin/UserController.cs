using Dapper;
using ELearnApp.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using WebApplication3.Validators;

namespace ELearnApp.Controllers.Admin;

[Route("/api/admin/[controller]/[action]")]
[IsInRole(UserRole.Admin)]
public class UserController : Controller
{
    private readonly MySqlConnection _mySqlConnection;

    public UserController(MySqlConnection mySqlConnection)
    {
        _mySqlConnection = mySqlConnection;
    }
    
    
    [HttpGet]
    public async Task<IActionResult> GetUserUID(string username)
    {
        var result = await _mySqlConnection.QueryFirstOrDefaultAsync<UserModel>("select user_uid from users where username = @username", new {username});

        if (result is UserModel user)
        {
            return Ok(user.user_uid);
        }
        return BadRequest("User Could not be found");
    }



}