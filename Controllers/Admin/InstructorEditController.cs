using System.ComponentModel.DataAnnotations;
using Dapper;
using ELearnApp.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using WebApplication3.Validators;

namespace ELearnApp.Controllers.Admin;

[IsInRole(UserRole.Admin)]
[Route("/api/admin/[controller]/[action]")]
public class InstructorEditController : Controller
{

    // Same as CourseEdit maybe change to SQL transactions
    
    private readonly MySqlConnection _mySqlConnection;
    
    public InstructorEditController(MySqlConnection mySqlConnection)
    {
        _mySqlConnection = mySqlConnection;
    }

    #region Put

    [HttpPut]
    public async Task<IActionResult> UpdateInstructor(UserModel userModel)
    {
        var validator = new InstructorEditValidator();

        var ValidatorResult = validator.Validate(userModel);
        if (!ValidatorResult.IsValid)
        {
            return BadRequest(ValidatorResult.Errors.FirstOrDefault());
        }
        
        var result = await _mySqlConnection.ExecuteAsync(
            "update users set full_name = @full_name, imgsrc = @imgsrc, bio = @bio, email = @email where user_uid = @user_uid",
            new {userModel.full_name, userModel.imgsrc, userModel.bio, userModel.email, userModel.user_uid});
        if (result != 1)
        {
            return BadRequest("Update failed");
        }

        return Ok();
        
        
        
    }

    [HttpPut]
    public async Task<IActionResult> AddInstructor([Required]string user_uid)
    {
        //Should i check if user is already instructor?
        var IsInstructor = await _mySqlConnection.QueryFirstOrDefaultAsync<UserModel>("Select role from users where user_uid = @user_uid",
            new {user_uid});

        if (IsInstructor is null || IsInstructor.role == UserRole.Instructor) return BadRequest("User is already instructor");
        
        
        
        
        var result = await _mySqlConnection.ExecuteAsync(
            "update users set role = 'instructor' where user_uid = @user_uid",
            new {user_uid});
        if (result != 1)
        {
            return BadRequest("Update failed");
        }

        return Ok();
        
        
        
    }
    
    
    #endregion
    
    #region Delete

    [HttpDelete]
    public async Task<IActionResult> RemoveInstructorRole(UserModel userModel)
    {
        var validator = new InstructorRemoveValidator();

        var ValidatorResult = validator.Validate(userModel);
        if (!ValidatorResult.IsValid)
        {
            return BadRequest(ValidatorResult.Errors.FirstOrDefault());
        }
        
        var result = await _mySqlConnection.ExecuteAsync(
            "update users set role = 'student' where user_uid = @user_uid",
            new {userModel.user_uid});
        if (result != 1)
        {
            return BadRequest("Update failed");
        }

        return Ok();
        
        
        
    }

    #endregion

    #region GET

    [HttpGet]
    public async Task<IActionResult> GetUserSuggestion([Required]string username)
    {

        username = $"{username}%";
        var result = await _mySqlConnection.QueryAsync<string>("Select username from users where username like @username limit 5",
            new {username});

        return new JsonResult(result);



    }

    #endregion

}