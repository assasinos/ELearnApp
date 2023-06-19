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

    #region HTTP PUT

    


    
    
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
    
    [HttpPut]
    public async Task<IActionResult> CreateLesson(LessonModel lessonModel)
    {
        
        
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
    
        
    [HttpPut]
    public async Task<IActionResult> CreateCourse([Required] CourseModel courseModel)
    {
        courseModel.Instructor = new(){user_uid = await User.GetUserUID()};
        
        
        var validator = new CourseCreationValidator();
        var ValidatorResult = await validator.ValidateAsync(courseModel);
        if (!ValidatorResult.IsValid)
        {
            return BadRequest(ValidatorResult.Errors.FirstOrDefault());
        }


        var result = await _mySqlConnection.QuerySingleAsync<string>(
            "insert into courses ( title, description, overview, imgsrc, instructor_uid) values (@title, @description, @overview, @imgsrc, @user_uid) RETURNING course_uid",
            new {courseModel.title, courseModel.description, courseModel.overview, courseModel.imgsrc, courseModel.Instructor.user_uid});
        if (string.IsNullOrWhiteSpace(result))
        {
            return BadRequest("Creation failed");
        }

        return Ok(result);
    }
    
    
    
    
    #endregion

    #region HTTP DELETE

    

    [HttpDelete]
    public async Task<IActionResult> DeleteLesson([Required] LessonModel lessonModel)
    {
        var validator = new LessonDeleteValidator();

        var validationResult = await validator.ValidateAsync(lessonModel);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.FirstOrDefault());
        }
        
        //Validate if the user is the instructor of the course
        var user_uid = await User.GetUserUID();

        var result = await _mySqlConnection.ExecuteAsync(
            "Delete l from lessons l join courses c on l.course_uid = c.course_uid where l.lesson_uid = @lesson_uid and l.course_uid = @course_uid and c.instructor_uid = @user_uid",
            new { lessonModel.lesson_uid, lessonModel.course_uid, user_uid});
        if (result != 1)
        {
            return BadRequest("Delete failed");
        }

        return Ok();
    }

    
    //Should i allow the instructors to delete the course?
    
    [HttpDelete]
    public async Task<IActionResult> DeleteCourse([Required] string course_uid)
    {
        //Ensure if the user is the instructor of the course
        var user_uid = await User.GetUserUID();
        if (await _mySqlConnection.QuerySingleAsync<int>("Select count(*) from courses c where c.instructor_uid = @user_uid and c.course_uid = @course_uid", new {user_uid, course_uid}) != 1)
        {
            return Forbid();
        }
        
        //Ensure Foreign key constraints are deleted
        await _mySqlConnection.ExecuteAsync(
            "Delete from lessons where course_uid = @course_uid",
            new {course_uid});
        
        await _mySqlConnection.ExecuteAsync(
            "Delete from user_courses where course_uid = @course_uid",
            new {course_uid});

        
        
        var result = await _mySqlConnection.ExecuteAsync(
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