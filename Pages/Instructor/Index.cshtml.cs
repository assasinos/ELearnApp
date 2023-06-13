using Dapper;
using ELearnApp.Extentions;
using ELearnApp.Models;
using ELearnApp.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace ELearnApp.Pages.Instructor;

[IsInRole(UserRole.Instructor)]
public class Index : PageModel
{
    private readonly MySqlConnection _mySqlConnection;


    public Index(MySqlConnection mySqlConnection)
    {
        _mySqlConnection = mySqlConnection;
    }


    internal IEnumerable<InstructorStatisticModel>? InstructorCoursesStatistics;

    public async Task<IActionResult> OnGet()
    {

        InstructorCoursesStatistics = await GetInstructorCoursesStatistics();
        
        
        return Page();
    }

    private async Task<IEnumerable<InstructorStatisticModel>?> GetInstructorCoursesStatistics()
    {
        var user_uid = await User.GetUserUID();

        var sql = new CommandDefinition("Select Count(user_courses.course_uid) as 'NumberOfStudents', courses.title , courses.imgsrc from courses join user_courses on courses.course_uid = user_courses.course_uid where instructor_uid = @user_uid group by user_courses.course_uid LIMIT 3 ", new {user_uid});


        return await _mySqlConnection.QueryAsync<InstructorStatisticModel, CourseModel, InstructorStatisticModel>(sql, (InstructorStatisticModel, CourseModel) =>
        {
            InstructorStatisticModel.Course = CourseModel;
            return InstructorStatisticModel;

        }, splitOn: "title");;
    }
}
