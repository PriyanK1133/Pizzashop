
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Pizzashop.Service.Interfaces;

namespace Pizzashop.Service.Implementations;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }
    public async Task<bool> SendEmailAsync(string emailAddress, string body, string subject)
    {
        bool isSent = false;
        try
        {

            var userName = _config["EmailConfiguration:UserName"];
            var port = int.Parse(_config["EmailConfiguration:Port"]!);
            var password = _config["EmailConfiguration:Password"];
            var host = _config["EmailConfiguration:Host"];
            var enableSSL = bool.Parse(_config["EmailConfiguration:EnableSSL"]!);


            using MailMessage mm = new(userName!, emailAddress);
            mm.Subject = subject;
            mm.Body = body;

            mm.IsBodyHtml = true;
            SmtpClient smtp = new(host, port)
            {
                EnableSsl = enableSSL,
                Credentials = new NetworkCredential(userName, password)
            };
            await smtp.SendMailAsync(mm);
            isSent = true;
        }
        catch (Exception)
        {
            isSent = false;
        }
        return isSent;
    }



    public string GetResetPasswordEmailBody(string resetLink)
    {
        string body = File.ReadAllText("wwwroot/EmailTemplates/ResetPassword.html");
        body = body.Replace("[RESETLINK]", resetLink);
        return body;
    }

    public string GetCreateUserEmailBody(string userName, string password)
    {
        string body = File.ReadAllText("wwwroot/EmailTemplates/CreatedUser.html");
        body = body.Replace("[USERNAME]", userName);
        body = body.Replace("[PASSWORD]", password);
        return body;
    }

}
