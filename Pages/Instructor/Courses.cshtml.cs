using Dapper;
using ELearnApp.Extentions;
using ELearnApp.Models;
using ELearnApp.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace ELearnApp.Pages.Instructor;

[IsInRole(UserRole.Instructor)]
public class Courses : PageModel
{
    private readonly MySqlConnection _mySqlConnection;


    public Courses(MySqlConnection mySqlConnection)
    {
        _mySqlConnection = mySqlConnection;
    }


    internal IEnumerable<CourseModel>? InstructorCourses;

    public async Task<IActionResult> OnGet()
    {

        InstructorCourses = await GetInstructorsCourses();
        
        
        return Page();
    }

    private async Task<IEnumerable<CourseModel>?> GetInstructorsCourses()
    {
        var user_uid = await User.GetUserUID();
        
        return await _mySqlConnection.QueryAsync<CourseModel>("Select title, imgsrc, course_uid from courses where instructor_uid = @user_uid order by created_at desc", new {user_uid});
        
        
    }
}