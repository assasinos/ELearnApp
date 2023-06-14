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
       
    [HttpPut]
    public async Task<IActionResult> UpdateLesson([Required] LessonModel lessonModel)
    {
        
        var validator = new LessonUpdateValidator();

        var validationResult = await validator.ValidateAsync(lessonModel);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.FirstOrDefault());
        }
        
        //For Validation if the user is the instructor of the course
        var user_uid = await User.GetUserUID();
        
        lessonModel.lesson_content = Converter.Convert(lessonModel.lesson_content);

        var result = await _mySqlConnection.ExecuteAsync(
            "update lessons l join courses c on c.course_uid = l.course_uid set l.lesson_name = @Lesson_name,l.lesson_content = @lesson_content where l.lesson_uid = @lesson_uid and l.course_uid = @course_uid " +
            "and c.instructor_uid = @user_uid",
            new { lessonModel.lesson_name, lessonModel.lesson_content, lessonModel.lesson_uid, lessonModel.course_uid, user_uid });
        if (result != 1)
        {
            return BadRequest("Update failed");
        }

        return Ok();
    }   
}