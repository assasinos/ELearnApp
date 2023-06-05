using System.ComponentModel.DataAnnotations;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ReverseMarkdown;
using WebApplication3.Models;
using WebApplication3.Validators;

namespace ELearnApp.Controllers.Admin;


[Route("/api/admin/[controller]/[action]")]
[IsInRole(UserRole.Admin)]
public class CourseEditController : Controller
{
    private readonly MySqlConnection _mySqlConnection;

    private static readonly Converter Converter = new();
    
    public CourseEditController(MySqlConnection connection)
    {
        _mySqlConnection = connection;
    }

    
    //Maybe change those to SQL transactions 

    #region Put

    [HttpPut]
    public async Task<IActionResult> CreateCourse([Required] CourseModel courseModel)
    {

        var validator = new CourseCreationValidator();
        var ValidatorResult = await validator.ValidateAsync(courseModel);
        if (!ValidatorResult.IsValid)
        {
            return BadRequest(ValidatorResult.Errors.FirstOrDefault());
        }


        var result = await _mySqlConnection.QuerySingleAsync<string>(
            "insert into courses ( title, description, overview, imgsrc, instructor_uid, featured) values (@title, @description, @overview, @imgsrc, @user_uid, @featured) RETURNING course_uid",
            new {courseModel.title, courseModel.description, courseModel.overview, courseModel.imgsrc, courseModel.Instructor.user_uid, courseModel.featured});
        if (string.IsNullOrWhiteSpace(result))
        {
            return BadRequest("Creation failed");
        }

        return Ok(result);
    }

 
    [HttpPut]
    public async Task<IActionResult> UpdateCourse([Required] CourseModel courseModel)
    {

        var validator = new CourseValidator();
        var ValidatorResult = await validator.ValidateAsync(courseModel);
        if (!ValidatorResult.IsValid)
        {
            return BadRequest(ValidatorResult.Errors.FirstOrDefault());
        }


        var result = await _mySqlConnection.ExecuteAsync(
            "update courses set title = @title, description = @description, imgsrc = @imgsrc,overview = @overview, featured = @featured, instructor_uid = @user_uid where course_uid = @course_uid",
            new {courseModel.title, courseModel.description, courseModel.imgsrc, courseModel.overview, courseModel.featured, courseModel.Instructor.user_uid, courseModel.course_uid});
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
        
        
        lessonModel.lesson_content = Converter.Convert(lessonModel.lesson_content);

        var result = await _mySqlConnection.ExecuteAsync(
            "update lessons set lesson_name = @Lesson_name,lesson_content = @lesson_content where lesson_uid = @lesson_uid and course_uid = @course_uid",
            new { lessonModel.lesson_name, lessonModel.lesson_content, lessonModel.lesson_uid, lessonModel.course_uid});
        if (result != 1)
        {
            return BadRequest("Update failed");
        }

        return Ok();
    }
    
    [HttpPut]
    public async Task<IActionResult> CreateLesson([Required] LessonModel lessonModel)
    {
        
        //I think lesson cannot be created empty (No Content or name)
        var validator = new LessonCreateValidator();

        var validationResult = await validator.ValidateAsync(lessonModel);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.FirstOrDefault());
        }
        
        
        lessonModel.lesson_content = Converter.Convert(lessonModel.lesson_content);
        
        
        var result = await _mySqlConnection.QuerySingleAsync<string>(
            "INSERT INTO lessons (`course_uid`,`lesson_name`,`lesson_content`) VALUES (@course_uid, @lesson_name,@lesson_content) RETURNING `lesson_uid`",
            new {lessonModel.course_uid, lessonModel.lesson_name, lessonModel.lesson_content});


        return Ok(result);
    }
    
    #endregion

    #region Delete

    


    [HttpDelete]
    public async Task<IActionResult> DeleteLesson([Required] LessonModel lessonModel)
    {
        var validator = new LessonDeleteValidator();

        var validationResult = await validator.ValidateAsync(lessonModel);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.FirstOrDefault());
        }
        

        var result = await _mySqlConnection.ExecuteAsync(
            "Delete from lessons where lesson_uid = @lesson_uid and course_uid = @course_uid",
            new { lessonModel.lesson_uid, lessonModel.course_uid});
        if (result != 1)
        {
            return BadRequest("Delete failed");
        }

        return Ok();
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteCourse([Required] string course_uid)
    {

        
        //Ensure Foreign key constraints are deleted
        var result = await _mySqlConnection.ExecuteAsync(
            "Delete from lessons where course_uid = @course_uid",
            new {course_uid});
        //Result here might be 0 if no lessons are found
        
        result = await _mySqlConnection.ExecuteAsync(
            "Delete from courses where course_uid = @course_uid",
            new {course_uid});
        if (result != 1)
        {
            return BadRequest("Delete failed");
        }

        return Ok();
    }


    #endregion
    
    
}