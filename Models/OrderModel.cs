namespace ELearnApp.Models;

public class OrderModel
{
    public string order_uid { get; set; }
    public string user_uid { get; set; }
    public string order_date { get; set; }
    public string total_amount { get; set; }
    public OrderStatus status { get; set; }
}

public enum OrderStatus
{
    Completed, 
    Pending, 
    Cancelled
}