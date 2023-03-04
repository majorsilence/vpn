using Dapper;
using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.AppSettings;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Helpers;

namespace Majorsilence.Vpn.BackgroundWorker;

public class EmailWorker : BackgroundService
{
    private IEmail email;
    private ILogger _logger;
    private string vpnConnectionString;

    public EmailWorker(ILogger logger, SmtpSettings smtp, IConfiguration configuration)
    {
        _logger = logger;
        email = new LiveEmail(smtp.FromAddress, smtp.Username, smtp.Password, smtp.Host, smtp.Port);
        vpnConnectionString = configuration.GetConnectionString("MySqlVpn");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var db = InitializeSettings.DbFactory)
                {
                    var queuedEmails = 
                        await db.QueryAsync<Poco.EmailWorkQueue>(
                            "SELECT * FROM EmailWorkQueue WHERE SentDateTimeUtc IS NULL ORDER BY ToSendDateUtc DESC");

                    foreach (var message in queuedEmails)
                    {
                        await email.SendMail(message.HtmlMessage, message.Subject, message.SendTo, true,
                            message.Attachment, EmailTemplates.Generic);
                        message.SentDateTimeUtc = DateTime.UtcNow;
                        await db.UpdateAsync(message);
                    }
                }

                // Check for emails to send every 30 seconds
                await Task.Delay(1000 * 30, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmailWorker: It appears the server setup failed");
            }
        }
    }
}