using Dapper;
using ELearnApp.Models;
using ELearnApp.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace ELearnApp.Pages.Admin;

[IsInRole(UserRole.Admin)]
public class Courses : PageModel
{
    internal MySqlConnection _mySqlConnection;

    public Courses(MySqlConnection mySqlConnection)
    {
        _mySqlConnection = mySqlConnection;
    }
    
    internal IEnumerable<CourseModel> courseModels;
    public async Task<IActionResult> OnGet( )
    {
        courseModels = await GetCourses();
        
        
        
        return Page();
    }

    private async Task<IEnumerable<CourseModel>> GetCourses()
    {

        return await _mySqlConnection.QueryAsync<CourseModel, UserModel, CourseModel>("SELECT `courses`.`course_uid`, `courses`.`title`,`courses`.`imgsrc`,`users`.`full_name`,`users`.`imgsrc` FROM `courses` JOIN `users` ON `users`.`user_uid` = `courses`.`instructor_uid` order by `users`.`created_at`;",(course, instructor) => {
                course.Instructor = instructor;
                return course;
            }, 
            splitOn: "full_name" );
    }
}