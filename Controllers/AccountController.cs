using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using Dapper;
using ELearnApp.Models;
using ELearnApp.Services.EmailServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Server.HttpSys;
using MySqlConnector;
using WebApplication3.Extentions;
using WebApplication3.Validators;

namespace WebApplication3.Controllers;

[Controller]
[Route("api/[controller]/[action]")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly MySqlConnection _mySqlConnection;

    private readonly IEmailService _emailService;
    public AccountController(ILogger<AccountController> logger,MySqlConnection connection, IEmailService emailService)
    {
        _logger = logger;
        _mySqlConnection = connection;
        _emailService = emailService;
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
        
        var validator = new RegistrationValidator();
        var ValidationResult = await validator.ValidateAsync(registerModel);
        if (!ValidationResult.IsValid)
        {
            return BadRequest(ValidationResult.Errors.FirstOrDefault()?.ErrorMessage);
        }
        
        
        //Check if username or email already is used

        var UsersResult = await _mySqlConnection.QueryFirstOrDefaultAsync<UserModel>("SELECT * FROM `users` WHERE `email` = @email or `username` = @username",
        new {registerModel.email, registerModel.username});
        var RegistrationResult = await _mySqlConnection.QueryFirstOrDefaultAsync<UserModel>("SELECT * FROM `registrations` WHERE `email` = @email or `username` = @username",
            new {registerModel.email, registerModel.username});
        if (UsersResult is not null || RegistrationResult is not null)
        {
            return BadRequest("User already exists");
        }

        registerModel.password = await registerModel.password.ComputeHash();

        var registrationuid = await _mySqlConnection.QuerySingleAsync<string>("INSERT INTO `registrations`(`username`, `password`, `email`, `full_name`) VALUES (@username,@password,@email,@full_name) RETURNING `registration_uid`", registerModel);
        
        //Send Email
        //TODO: Make Mail body prettier
        _emailService.SendEmail(registerModel.email, "Confirm your email", $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>Account Confirmation</title>
            </head>
            <body>
            <h1>Account Confirmation</h1>
            <p>Dear {registerModel.full_name},</p>
            <p>Thank you for creating an account. To activate your account, please click the link below:</p>
            <p><a href="https://{Request.Host.Value}/Profile/ConfirmAccount?token={registrationuid}">Activate Account</a></p>
            <p>If you did not create this account, please ignore this email.</p>
            <p>Best regards,</p>
            <p>ELearnapp</p>
            </body>
            </html>
            """);
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

    [IsAuthenticated]
    [HttpDelete]
    public async Task<IActionResult> DeleteAccount(string confirmpassword)
    {
        if (confirmpassword is null)
        {
            return BadRequest("Password cannot be empty");
        }
        
        
        var user_uid = await User.GetUserUID();
        
        //Check if password is correct
        var user = await _mySqlConnection.QuerySingleOrDefaultAsync<UserModel>("Select password,role from users where  user_uid = @user_uid", new {user_uid});
        
        
        if (user.password != await confirmpassword.ComputeHash()) return BadRequest("Password is incorrect");

        
        
        //Constrains
        // Maybe add some flag and after certain time delete account and it's orders
        await _mySqlConnection.ExecuteAsync("delete from orders where user_uid = @user_uid", new { user_uid });
        await _mySqlConnection.ExecuteAsync("delete from user_courses where user_uid = @user_uid", new { user_uid });
        //Maybe disallow removal of admin account
        if (user.role == UserRole.Instructor)
        {
            var adminUID =
                await _mySqlConnection.QuerySingleOrDefaultAsync<string>(
                    "Select user_uid from users where role = 'admin' LIMIT 1");

            await _mySqlConnection.ExecuteAsync("Update courses set instructor_uid = @adminUID where instructor_uid = @user_uid", new { adminUID, user_uid });
        }
        
        
        var result =
            await _mySqlConnection.ExecuteAsync("Delete from users where user_uid = @user_uid", new { user_uid });

        if (result == 0)
        {
            return BadRequest("Something went Wrong");
        }

        return Ok();
    }



}