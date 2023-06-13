using Dapper;
using ELearnApp.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using WebApplication3.Validators;

namespace ELearnApp.Controllers.Admin;


[IsInRole(UserRole.Admin)]
[Route("/api/admin/[controller]/[action]")]
public class OrdersController : Controller
{
    private readonly ILogger<OrdersController> _logger;
    private readonly MySqlConnection _mySqlConnection;

    public OrdersController(ILogger<OrdersController> logger,MySqlConnection connection)
    {
        _logger = logger;
        _mySqlConnection = connection;
    }
    
    [ProducesResponseType(typeof(IEnumerable<ChartDataModel>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [HttpGet]
    public async Task<IActionResult> GetChartData()
    {

        
        //Less than 5 months
        //Maybe shouldn't return any last 5 months but last 5 months with orders
        //bcs it makes chart.js look weird and kinda empty if there are no orders in last 5 months
        var result = await _mySqlConnection.QueryAsync<ChartDataModel>(
            "SELECT sum(`total_amount`) AS 'total_amount', MONTHNAME(`order_date`) as 'Month' FROM `orders` WHERE `status` = 'Completed' and PERIOD_DIFF(DATE_FORMAT(CURRENT_DATE(), '%y%m'),DATE_FORMAT(`order_date`, '%y%m'))< 5 GROUP BY DATE_FORMAT(`order_date`, '%m.%y');");
        
        

        return new JsonResult(result.AsList());
    }

    
    private class ChartDataModel
    {
        public decimal total_amount { get; set; }
        public string Month { get; set; }
    
    }


}

