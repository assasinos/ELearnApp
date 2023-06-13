using Dapper;
using ELearnApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using WebApplication3.Controllers;

namespace ELearnApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger, MySqlConnection connection)
    {
        _logger = logger;
        _mySqlConnection = connection;
    }

    internal IEnumerable<CourseModel> Featured;
    private MySqlConnection _mySqlConnection;


    public async Task<IActionResult> OnGetAsync()
    {
        Featured = await GetFeatured();
        return Page();
    }
    
    
    
    internal async Task<IEnumerable<CourseModel>> GetFeatured()
    {
        var result = await _mySqlConnection.QueryAsync<CourseModel>("SELECT `course_uid`, `title`, `description`, `imgsrc` FROM `courses` where `featured` = 1 ");
        return result;
    }
}