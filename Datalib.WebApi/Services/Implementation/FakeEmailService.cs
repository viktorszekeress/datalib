namespace Datalib.WebApi.Services.Implementation;

public class FakeEmailService : IEmailService
{
    private readonly ILogger<FakeEmailService> _logger;

    public FakeEmailService(ILogger<FakeEmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendEmailAsync(string address, string subject, string body)
    {
        _logger.LogCritical("Sending email to {address}...", address);
        await Task.Delay(1_000);
        _logger.LogCritical("Subject: {subject}", subject);
        _logger.LogCritical("Body: {body}", body);

        await Task.Delay(2_000);

        _logger.LogCritical("Email sent to {address} at: {time}", address, DateTimeOffset.Now);
        _logger.LogCritical(Environment.NewLine);
    }
}