namespace ELearnApp.Models;

public class UserModel
{
    public string user_uid { get; set; }
    public string username { get; set; }
    public string password { get; set; }
    public string email { get; set; }
    public string full_name { get; set; }
    public UserRole role { get; set; }
    public string imgsrc { get; set; }
    public string bio { get; set; }
    public DateTime created_at { get; set; }
    
}

public enum UserRole
{
    Student,
    Instructor, 
    Admin
}