using System.Collections;
using Dapper;
using ELearnApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace ELearnApp.Pages.Course;

public class Index : PageModel
{
    
    internal IEnumerable<CourseModel> Courses;
    private MySqlConnection _mySqlConnection;
    private readonly ILogger<Index> _logger;

    public Index(ILogger<Index> logger, MySqlConnection connection)
    {
        _logger = logger;
        _mySqlConnection = connection;
    }
    
    public async Task<IActionResult> OnGet()
    {

        Courses = await GetCoursesDisplayData();
        return Page();
    }

    
    
    
    private async Task<IEnumerable<CourseModel>> GetCoursesDisplayData()
    {
        var result = await _mySqlConnection.QueryAsync<CourseModel>("SELECT `course_uid`, `title`, `description`, `imgsrc` FROM `courses`");

        return result;
    }
    
    
    
    
}