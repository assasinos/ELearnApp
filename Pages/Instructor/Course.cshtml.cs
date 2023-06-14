using Dapper;
using ELearnApp.Extentions;
using ELearnApp.Models;
using ELearnApp.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace ELearnApp.Pages.Instructor;

[IsInRole(UserRole.Instructor)]
public class Course : PageModel
{
    private readonly MySqlConnection _mySqlConnection;

    public Course(MySqlConnection mySqlConnection)
    {
        _mySqlConnection = mySqlConnection;
    }

    internal CourseModel? CourseModel;
    //Maybe replace it with a list of lessons inside of course Model
    internal IEnumerable<LessonModel>? LessonModels;


    public async Task<IActionResult> OnGet(string course_uid)
    {
        CourseModel = await GetCourse(course_uid);

        if (CourseModel is null) return NotFound();

        LessonModels = await GetLessons(course_uid);

        return Page();
    }

    private async Task<IEnumerable<LessonModel>?> GetLessons(string courseUid)
    {
        return await _mySqlConnection.QueryAsync<LessonModel>("Select lesson_content,lesson_name,lesson_uid from lessons where course_uid = @courseUid", new { courseUid });
    }

    private async Task<CourseModel?> GetCourse(string courseUid)
    {
        var user_uid = await User.GetUserUID();



        return await _mySqlConnection.QueryFirstOrDefaultAsync<CourseModel>(
            "SELECT course_uid, created_at, description, featured, overview, title, imgsrc FROM courses WHERE courses.course_uid = @courseUid and courses.instructor_uid = @user_uid",
            new { uid = courseUid });
    }
}