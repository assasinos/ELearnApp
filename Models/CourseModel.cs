namespace WebApplication3.Models;

public class CourseModel
{
    public string course_uid { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public string overview { get; set; }
    public string imgsrc { get; set; }
    public UserModel Instructor { get; set; }
    
    public DateTime created_at { get; set; }
    public bool featured { get; set; } = false;
}

