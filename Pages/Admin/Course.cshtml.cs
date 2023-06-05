using Dapper;
using ELearnApp.Pages.Error;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using WebApplication3.Models;
using WebApplication3.Validators;

namespace ELearnApp.Pages.Admin;

[IsInRole(UserRole.Admin)]
public class Course : PageModel
{
    internal MySqlConnection _mySqlConnection;


    public Course(MySqlConnection mySqlConnection)
    {
        _mySqlConnection = mySqlConnection;
    }


    internal CourseModel? CourseModel;
    internal IEnumerable<UserModel> Instructors;
    internal IEnumerable<LessonModel> LessonModels;
    public async Task<IActionResult> OnGetAsync(string course_uid)
    {

        var result = await GetCourseInfo(course_uid);

        if (result is null)
        {
            return NotFound();
        }
        
        CourseModel = result;
        Instructors = await GetAllInstructors(CourseModel.Instructor.user_uid);
        LessonModels = await GetLessons(course_uid);
        return Page();
    }

    private async Task<IEnumerable<LessonModel>> GetLessons(string course_uid)
    {
        return await _mySqlConnection.QueryAsync<LessonModel>("SELECT * FROM lessons WHERE course_uid = @course_uid", new {course_uid});
    }

    private async Task<IEnumerable<UserModel>> GetAllInstructors(string instructor_uid)
    {
        return await _mySqlConnection.QueryAsync<UserModel>("SELECT full_name,user_uid FROM users WHERE role = 'instructor' AND user_uid != @instructor_uid", new {instructor_uid});
    }

    private async Task<CourseModel?> GetCourseInfo(string course_uid)
    {
        var sqlCommand = new CommandDefinition("SELECT courses.course_uid, courses.created_at, courses.description, courses.featured, courses.imgsrc, courses.overview, courses.title, users.user_uid, users.full_name FROM courses join users on courses.instructor_uid = users.user_uid WHERE courses.course_uid = @uid", new {uid = course_uid});
             
        var result = await _mySqlConnection.QueryAsync<CourseModel, UserModel,CourseModel>(sqlCommand, (course, instructor) =>
        {
            course.Instructor = instructor;
            return course;
        },
            splitOn: "user_uid" );

        return result.FirstOrDefault();
    }
}