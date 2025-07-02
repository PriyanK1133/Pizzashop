namespace Pizzashop.Service.Interfaces;

public interface IEmailService
{
    public Task<bool> SendEmailAsync(string emailAddress, string body, string subject);
    public string GetResetPasswordEmailBody(string resetLink);
    public string GetCreateUserEmailBody(string userName, string password);
}
