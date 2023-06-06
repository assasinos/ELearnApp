using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Server.HttpSys;
using MySqlConnector;
using WebApplication3.Extentions;
using WebApplication3.Models;
using WebApplication3.Validators;

namespace WebApplication3.Controllers;

[Controller]
[Route("api/[controller]/[action]")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly MySqlConnection _mySqlConnection;

    public AccountController(ILogger<AccountController> logger,MySqlConnection connection)
    {
        _logger = logger;
        _mySqlConnection = connection;
    }

    [HttpPost]
    [ProducesResponseType(typeof(OkResult), 200)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [IsNotAuthenticated]
    public async Task<IActionResult> login( [Required] string username, [Required] string password )
    {


        password = await password.ComputeHash();
        var result = _mySqlConnection.Query<UserModel>("select `user_uid`, `role` from `users` where `username` = @username and `password` = @password", new {username,password});
    
        
        //There should be only one value returned
        if (result.SingleOrDefault() is UserModel user)
        {
            var claims = new List<Claim>();
            claims.Add(new ("user_uid", user.user_uid.ToString()));
            claims.Add(new (ClaimTypes.Role, user.role.ToString()));
            var identity = new ClaimsIdentity(claims, "cookie");
            var login = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(login);
            return Ok();
        }
    
        return Problem(
            detail: $"Username or password is wrong",
            instance: HttpContext.Request.Path,
            statusCode: 400,
            title: "Sign in failed");
    
    }
    
    
    [HttpPost]
    [ProducesResponseType(typeof(OkResult), 200)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [IsNotAuthenticated]
    public async Task<IActionResult> Register([Required] RegisterModel registerModel)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.FirstOrDefault(e=> e.Value.ValidationState == ModelValidationState.Invalid)
                .Value.Errors.FirstOrDefault().ErrorMessage);
        }
        
        
        //Check if username or email already is used

        var result = await _mySqlConnection.QueryAsync<UserModel>("SELECT * FROM `users` WHERE `email` = @email or `username` = @username",
        new {registerModel.email, registerModel.username});

        if (result.FirstOrDefault() is not null)
        {
            return BadRequest("Username or email is already in use");
        }

        registerModel.password = await registerModel.password.ComputeHash();

        await _mySqlConnection.ExecuteAsync("INSERT INTO `users`(`username`, `password`, `email`, `full_name`) VALUES (@username,@password,@email,@full_name)", registerModel);
        //Add e-mail in future
        
        return Ok();
    }

    [HttpPost]
    [ProducesResponseType(typeof(OkResult), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [IsAuthenticated]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return Ok();
    }

    [IsAuthenticated]
    [ProducesResponseType(typeof(OkResult), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [HttpPut]
    public async Task<IActionResult> UpdatePersonalInfo(UserModel userModel)
    {

        var validator = new PersonalInfoValidator();
        var ValidationResult = await validator.ValidateAsync(userModel);
        if (!ValidationResult.IsValid)
        {
            return BadRequest(ValidationResult.Errors.FirstOrDefault().ErrorMessage);
        }


        var user_uid = await User.GetUserUID();
        
        //Check if password is correct
        var password = await _mySqlConnection.QuerySingleOrDefaultAsync<string>("Select password from users where  user_uid = @user_uid", new {user_uid});
        
        
        if (password != await userModel.password.ComputeHash()) return BadRequest("Password is incorrect");
        
        //Update Info
        
        var result = await _mySqlConnection.ExecuteAsync("UPDATE `users` SET `full_name` = @full_name, `email` = @email WHERE `user_uid` = @user_uid", new {userModel.full_name, userModel.email, user_uid});

        if (result != 1)
        {
            return BadRequest("Something went wrong");
        }
        
        return Ok();
    }

    [IsAuthenticated]
    [HttpPatch]
    public async Task<IActionResult> UpdatePassword(string OldPassword, string NewPassword)
    {

        if (NewPassword is null || OldPassword is null)
        {
            return BadRequest("Password cannot be empty");
        }
        
        
        var user_uid = await User.GetUserUID();
        
        //Check if password is correct
        var password = await _mySqlConnection.QuerySingleOrDefaultAsync<string>("Select password from users where  user_uid = @user_uid", new {user_uid});
        
        
        if (password != await OldPassword.ComputeHash()) return BadRequest("Password is incorrect");
        
        
        //update password
        NewPassword = await NewPassword.ComputeHash();
        var result = await _mySqlConnection.ExecuteAsync("UPDATE `users` SET `password` = @NewPassword WHERE `user_uid` = @user_uid", new {NewPassword, user_uid});

        if (result != 1)
        {
            return BadRequest("Something went wrong");
        }

        return Ok();
    }




}