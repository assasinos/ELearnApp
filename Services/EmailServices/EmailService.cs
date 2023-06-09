
using System.Net;
using System.Net.Mail;

namespace ELearnApp.Services.EmailServices;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;


    public EmailService(IConfiguration configuration)
    {
        _config = configuration.GetSection("MailSettings");
    }
    
    
    public void SendEmail(string to, string subject, string body)
    {
        var message = new MailMessage(_config["Mail"], to, subject, body);
        message.IsBodyHtml = true;
        using var client = new SmtpClient(_config["Host"], int.Parse(_config["Port"] ?? throw new InvalidOperationException()))
        {
            Credentials = new NetworkCredential(_config["Mail"], _config["Password"]),
            EnableSsl = _config["EnableSsl"] == "true"
        };
        client.Send(message);
    }
}