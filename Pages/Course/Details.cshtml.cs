using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using WebApplication3.Models;

namespace ELearnApp.Pages.Course;

public class Details : PageModel
{
    
    private MySqlConnection _mySqlConnection;
    private readonly ILogger<Details> _logger;

    public Details(ILogger<Details> logger, MySqlConnection connection)
    {
        _logger = logger;
        _mySqlConnection = connection;
    }
    
    
    
    public async Task<IActionResult> OnGet(string course_uid)
    {

        Course = await GetCourseData(course_uid);

        if (Course is null)
        {
            return NotFound();
        }

        Lessons = await GetLessonsData(course_uid);
        
        return Page();
    }

    internal CourseModel? Course;
    internal IEnumerable<LessonModel> Lessons;


    private async Task<IEnumerable<LessonModel>> GetLessonsData(string courseuid)
    {
        
        
        return await _mySqlConnection.QueryAsync<LessonModel>(
            "SELECT `lesson_name` FROM `lessons` WHERE `course_uid` = @courseuid", new {courseuid});
            
    }
    
    
    
    private async Task<CourseModel?> GetCourseData(string courseuid)
    {
        var sql = new CommandDefinition("SELECT `courses`.`course_uid`,`courses`.`title`,`courses`.`description`, `courses`.`overview`, `courses`.`imgsrc`, `users`.`full_name`, `users`.`imgsrc`, `users`.`bio`  FROM `courses` INNER JOIN `users` ON `courses`.`instructor_uid` = `users`.`user_uid` WHERE `courses`.`course_uid` = @courseuid"
            , new {courseuid});
				
        var result = await _mySqlConnection.QueryAsync<CourseModel, UserModel, CourseModel>(sql,(course, instructor) => {
                course.Instructor = instructor;
                return course;
            }, 
            splitOn: "full_name" );
        return result.FirstOrDefault();
    }
}