using System.ComponentModel.DataAnnotations;
using Dapper;
using ELearnApp.Models;
using ELearnApp.Validators;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace ELearnApp.Controllers;

[Route("api/[controller]/[action]")]
public class LessonController : Controller
{
    
    private readonly ILogger<LessonController> _logger;
    private readonly MySqlConnection _mySqlConnection;

    public LessonController(ILogger<LessonController> logger,MySqlConnection connection)
    {
        _logger = logger;
        _mySqlConnection = connection;
    }

    [HttpGet]
    [ProducesResponseType(typeof(LessonModel), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]

    public async Task<IActionResult> GetLessonData([Required] string lessonuid,
        [Required] string courseuid)
    {
        var user_uid = User.FindFirst("user_uid").Value;
        var acccess = await _mySqlConnection.QueryAsync<UserCoursesModel>(
            "SELECT * FROM `user_courses` WHERE `user_courses`.`user_uid` = @user_uid and `user_courses`.`course_uid` = @courseuid",
            new { user_uid, courseuid });

        if (acccess.SingleOrDefault() is null && !User.IsInRole(UserRole.Admin.ToString())) return Unauthorized();
        
        
        //To ensure that requester know if the lesson is in specified course
        var sql = new CommandDefinition($"SELECT `lesson_name`,`lesson_content` FROM `lessons` WHERE `course_uid` = @courseuid and lesson_uid = @lessonuid "
            , new {courseuid,lessonuid});
        
        var result = await _mySqlConnection.QuerySingleOrDefaultAsync<LessonModel>(sql); 
        //check if course was returned
        if (result is LessonModel lesson )
        {
            
            lesson.lesson_content = Markdig.Markdown.ToHtml(lesson.lesson_content);
            
            return new JsonResult(lesson);
        }

        return Problem(
            detail: $"Unable to retrieve lesson with id {lessonuid}",
            instance: HttpContext.Request.Path,
            statusCode: 404,
            title: "An error occurred");
        
        
    }

    [HttpGet]
    [ProducesResponseType(typeof(CourseModel), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [IsAuthenticated]
    public async Task<IActionResult> GetLessonPageData([Required][Range(1,int.MaxValue)]int LessonNumber,[Required]string courseuid)
    {
        var user_uid = User.FindFirst("user_uid").Value;
        var acccess = await _mySqlConnection.QueryAsync<UserCoursesModel>("SELECT * FROM `user_courses` WHERE `user_courses`.`user_uid` = @user_uid and `user_courses`.`course_uid` = @courseuid", new{user_uid,courseuid});

        if (acccess.SingleOrDefault() is null ) return Unauthorized();
        
        LessonNumber -= 1;
        
        //Might be a better way to do this
        //Maybe much more secure
        //But this is the best I can do for now
        var sql = new CommandDefinition($"SELECT `lesson_name`,`lesson_content` FROM `lessons` WHERE `lessons`.`course_uid` = @courseuid limit 1 offset {LessonNumber} "
            , new {courseuid,LessonNumber});
                
        var result = await _mySqlConnection.QuerySingleOrDefaultAsync<LessonModel>(sql); 
        //check if course was returned
        if (result is LessonModel lesson )
        {
            
            lesson.lesson_content = Markdig.Markdown.ToHtml(lesson.lesson_content);
            
            return new JsonResult(lesson);
        }

        return Problem(
            detail: $"Unable to retrieve lesson with id {LessonNumber}",
            instance: HttpContext.Request.Path,
            statusCode: 404,
            title: "An error occurred");
    }

}
