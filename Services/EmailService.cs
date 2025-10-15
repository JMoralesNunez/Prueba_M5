using System.Net;
using System.Net.Mail;

namespace Prueba.Services;

public interface IEmailService
{
    Task SendEmail(string to, string subject, string message);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmail(string to, string subject, string body)
    {
        var fromEmail = _configuration.GetValue<string>("EMAIL_CONFIGURATIONS:From");
        var password = _configuration.GetValue<string>("EMAIL_CONFIGURATIONS:Password");
        var host = _configuration.GetValue<string>("EMAIL_CONFIGURATIONS:Host");
        var port = _configuration.GetValue<int>("EMAIL_CONFIGURATIONS:Port");
        
        var smtp = new SmtpClient(host, port);
        smtp.EnableSsl = true;
        smtp.UseDefaultCredentials = false;
        
        smtp.Credentials = new NetworkCredential(fromEmail, password);
        var message = new MailMessage(fromEmail!, to,  subject, body);
        message.IsBodyHtml = true;
        
        Console.WriteLine($"SMTP config: {fromEmail}, {host}, {port}, {password.Length} chars");

        await smtp.SendMailAsync(message);
    }
}