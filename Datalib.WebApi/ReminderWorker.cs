using System.Text;
using Datalib.WebApi.Services;

namespace Datalib.WebApi;

public class ReminderWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReminderWorker> _logger;

    public ReminderWorker(IServiceProvider serviceProvider, ILogger<ReminderWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("ReminderWorker running at: {currentTime}", DateTime.Now.ToString("u"));

            using (var scope = _serviceProvider.CreateScope())
            {
                var checkoutService = scope.ServiceProvider.GetRequiredService<ICheckoutService>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                try
                {
                    var infos = await checkoutService.GetItemToRemindAsync();

                    // TODO: Could be improved by avoiding to send duplicates...

                    foreach (var info in infos)
                    {
                        var subject = $"Reminder about checkout from {info.IssuedOn:D}";
                        var body = new StringBuilder($"Just a kind reminder, that these titles need to returned: {Environment.NewLine}");
                        info.AuthorsAndTitles.ForEach(x => body.AppendLine(x));
                        body.AppendLine("Have a nice day!");

                        await emailService.SendEmailAsync(info.Email, subject, body.ToString());
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("Error detecting reminders: {message}", e.Message);
                }
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}