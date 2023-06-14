using System.ComponentModel.DataAnnotations;
using Dapper;
using ELearnApp.Extentions;
using ELearnApp.Models;
using ELearnApp.Validators;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ReverseMarkdown;

namespace ELearnApp.Controllers.Instructor;

[Route("/api/instructor/[controller]/[action]")]
[IsInRole(UserRole.Instructor)]
public class CourseEditController : Controller
{
    private readonly MySqlConnection _mySqlConnection;
    private static readonly Converter Converter = new();
    public CourseEditController(MySqlConnection sqlConnection)
    {
        _mySqlConnection = sqlConnection;
    }
    
    
    [HttpPut]
    public async Task<IActionResult> UpdateCourse([Required] CourseModel courseModel)
    {
        //For Validator
        courseModel.Instructor = new (){user_uid = await User.GetUserUID()};
        
        var validator = new CourseValidator();
        var validatorResult = await validator.ValidateAsync(courseModel);
        if (!validatorResult.IsValid)
        {
            return BadRequest(validatorResult.Errors.FirstOrDefault());
        }


        var result = await _mySqlConnection.ExecuteAsync(
            "update courses set title = @title, description = @description, imgsrc = @imgsrc,overview = @overview where course_uid = @course_uid and instructor_uid = @user_uid",
            new {courseModel.title, courseModel.description, courseModel.imgsrc, courseModel.overview, courseModel.course_uid, courseModel.Instructor.user_uid});
        if (result != 1)
        {
            return BadRequest("Update failed");
        }

        return Ok();
    }
 
}