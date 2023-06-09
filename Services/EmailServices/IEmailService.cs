namespace ELearnApp.Services.EmailServices;

public interface IEmailService
{
    void SendEmail(string to, string subject, string body);
}