namespace Datalib.WebApi.Services;

public interface IEmailService
{
    Task SendEmailAsync(string address, string subject, string body);
}