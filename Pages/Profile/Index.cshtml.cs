
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using WebApplication3.Models;

namespace ELearnApp.Pages.Profile;
public class Index : PageModel
{
    internal UserModel UserInfo;
    internal IEnumerable<CourseModel> UserCourses;
    private MySqlConnection _mySqlConnection;
    private readonly ILogger<Index> _logger;

    internal int CoursesCount;
    internal int CoursesPerPage = 6;

    public Index(ILogger<Index> logger, MySqlConnection connection)
    {
        _logger = logger;
        _mySqlConnection = connection;
    }

    public async Task<IActionResult> OnGet()
    {
        var user_uid = User.FindFirst("user_uid")?.Value;
        if (user_uid is null) return RedirectToPage("/Index");
        UserInfo = await GetUser(user_uid);
        UserCourses = await GetUserCourses(user_uid);
        
        return Page();
    }

    private async Task<IEnumerable<CourseModel>> GetUserCourses(string userUid)
    {
        CoursesCount = await _mySqlConnection.QuerySingleAsync<int>("SELECT COUNT(*) FROM `user_courses` WHERE `user_uid` = @userUid", new {userUid});
        
        
        
        
        return await _mySqlConnection.QueryAsync<CourseModel>(
            "SELECT `courses`.`course_uid`, `courses`.`title`, `courses`.`imgsrc`  FROM `user_courses` JOIN `courses` on `user_courses`.`course_uid` = `courses`.`course_uid` WHERE `user_uid` = @userUid limit 1",
            new { userUid });

    }
    


    private async Task<UserModel> GetUser(string useruid)
    {
        var result = await _mySqlConnection.QuerySingleAsync<UserModel>(
            "SELECT `role`,`full_name`,`email`,`imgsrc` from `users` WHERE `user_uid` = @useruid", new { useruid });

        
        //Shouldn't be null
        return result;
    }
}