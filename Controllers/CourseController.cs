using System.ComponentModel.DataAnnotations;
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
public class CourseController :ControllerBase
{
    private readonly ILogger<CourseController> _logger;
    private readonly MySqlConnection _mySqlConnection;

    public CourseController(ILogger<CourseController> logger,MySqlConnection connection)
    {
        _logger = logger;
        _mySqlConnection = connection;
    }


    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CourseModel>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [IsAuthenticated]
    public async Task<IActionResult> GetUserCourses([Required][Range(1,int.MaxValue)] int pageNumber)
    {

        var userUid = User.FindFirst("user_uid").Value;

        var offset = (pageNumber - 1) * 1;
        
        Console.Write(offset);
        
        return new JsonResult(await _mySqlConnection.QueryAsync<CourseModel>(
            $"SELECT `courses`.`course_uid`, `courses`.`title`, `courses`.`imgsrc` FROM `user_courses` JOIN `courses` on `user_courses`.`course_uid` = `courses`.`course_uid` WHERE `user_uid` = @useruid limit 1 OFFSET {offset}",
            new { userUid }));


    }



    [HttpGet]
    [ProducesResponseType(typeof(CourseModel), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [IsAuthenticated]
    public async Task<IActionResult> GetCourseData([Required]string courseuid)
    {
        var user_uid = User.FindFirst("user_uid").Value;
        var acccess = await _mySqlConnection.QueryAsync<UserCoursesModel>("SELECT * FROM `user_courses` WHERE `user_courses`.`user_uid` = @user_uid and `user_courses`.`course_uid` = @courseuid", new{user_uid,courseuid});

        if (acccess.SingleOrDefault() is null) return Unauthorized();
        
        var sql = new CommandDefinition("SELECT `courses`.`course_uid`,`courses`.`title`,`courses`.`description`, `courses`.`overview`, `users`.`full_name`,`courses`.`imgsrc`, `users`.`imgsrc` ,`users`.`bio`  FROM `courses` INNER JOIN `users` ON `courses`.`instructor_uid` = `users`.`user_uid` WHERE `courses`.`course_uid` = @courseuid"
        , new {courseuid});
				
        var result = await _mySqlConnection.QueryAsync<CourseModel, UserModel, CourseModel>(sql,(course, instructor) => {
                course.Instructor = instructor;
                return course;
            }, 
            splitOn: "full_name" ); 
        //check if course was returned
        if (result.SingleOrDefault() is CourseModel user )
        {
            return new JsonResult(user);
        }

        return Problem(
            detail: $"Unable to retrieve course with id {courseuid}",
            instance: HttpContext.Request.Path,
            statusCode: 404,
            title: "An error occurred");
    }


    
    
    
    
    
    
    
}

