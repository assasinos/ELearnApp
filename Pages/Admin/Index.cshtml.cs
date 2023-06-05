using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using WebApplication3.Models;
using WebApplication3.Validators;

namespace ELearnApp.Pages.Admin;

[IsInRole(UserRole.Admin)]
public class Index : PageModel
{
    internal MySqlConnection _mySqlConnection;

    public Index(MySqlConnection mySqlConnection)
    {
        _mySqlConnection = mySqlConnection;
    }
    
    
    internal IEnumerable<UserModel> userModels;
    internal IEnumerable<OrderModel> LastOrders;
    public async Task<IActionResult> OnGet( )
    {
        
        
        userModels = await GetUsers();
        LastOrders = await GetLastOrders();
        return Page();
    }

    private async Task<IEnumerable<OrderModel>> GetLastOrders()
    {
        return await _mySqlConnection.QueryAsync<OrderModel>("SELECT `order_uid`,`total_amount` FROM `orders` WHERE `status` = 'Completed' ORDER BY `order_date` DESC LIMIT 5;");
    }

    private async Task<IEnumerable<UserModel>> GetUsers()
    {
        return await _mySqlConnection.QueryAsync<UserModel>("SELECT `full_name`,`email`,`imgsrc` FROM `users` ORDER BY `created_at` LIMIT 3");
    }
}

