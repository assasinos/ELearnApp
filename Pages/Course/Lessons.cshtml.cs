using Dapper;
using ELearnApp.Models;
using ELearnApp.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace ELearnApp.Pages.Course;


[IsAuthenticated]
public class Lessons : PageModel
{
    
    private MySqlConnection _mySqlConnection;
    private readonly ILogger<Lessons> _logger;

    internal LessonModel lessonModel;

    public Lessons(ILogger<Lessons> logger, MySqlConnection connection)
    {
        _logger = logger;
        _mySqlConnection = connection;
    }

    internal int LessonCount;


    public async Task<IActionResult> OnGet(string course_uid)
    {
        var useruid = User.FindFirst("user_uid")?.Value;

        if (!await UserHasAccess(useruid, course_uid)) return Unauthorized();

        lessonModel = await GetFirstLesson(course_uid);
        lessonModel.lesson_content = Markdig.Markdown.ToHtml(lessonModel.lesson_content);
        LessonCount = await _mySqlConnection.QuerySingleAsync<int>("SELECT COUNT(*) FROM `lessons` WHERE `course_uid` = @course_uid", new {course_uid});
        
            return Page();
    }

    private async Task<LessonModel> GetFirstLesson(string courseUid)
    {
        //It can be done more safely probably
        
        return await _mySqlConnection.QuerySingleAsync<LessonModel>("SELECT * FROM `lessons` WHERE `course_uid` = @courseUid LIMIT 1", new {courseUid});
    }

    private async Task<bool> UserHasAccess(string? useruid, string courseUid)
    {
        var result = await _mySqlConnection.QueryAsync<UserCoursesModel>("SELECT `user_uid` FROM `user_courses` WHERE `user_uid` = @useruid and `course_uid` = @courseUid", new {useruid, courseUid});

        return result.FirstOrDefault() is not null;
    }
}