using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Logic.Helpers;

namespace Majorsilence.Vpn.Logic.Email;

public class EmailWorkQueue : IEmail
{
    public async Task SendMail(string message, string subject, string to,
        bool isHtml, byte[] attachment = null, EmailTemplates template = EmailTemplates.None)
    {
        // TODO: replace with kafka, redis streams, or something similar
        using var db = InitializeSettings.DbFactory;
        db.Open();
        
        var sendMessage = message;
        if (template != EmailTemplates.None) sendMessage = ProcessMessage(template, message, subject);
        var email = new Poco.EmailWorkQueue()
        {
            HtmlMessage = sendMessage,
            Attachment = attachment,
            PlainTextMessage = "",
            SendTo = to,
            Subject = subject,
            SentDateTimeUtc = null,
            ToSendDateUtc = DateTime.UtcNow
        };
        await db.InsertAsync(email);
    }

    private string ProcessMessage(EmailTemplates template, string message, string subject)
    {
        var output = LoadTemplate(template);

        output = output.Replace("{footer}", string.Format("For more information please visit <a href=\"{0}\">{1}</a>",
            SiteInfo.SiteUrl, SiteInfo.SiteName));

        if (template == EmailTemplates.BetaKey || template == EmailTemplates.Generic)
        {
            output = output.Replace("{subject}", subject);
            output = output.Replace("{message}", message);

            return output;
        }

        return message;
    }

    private string LoadTemplate(EmailTemplates template)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "";

        if (template == EmailTemplates.BetaKey)
            resourceName = "Majorsilence.Vpn.Logic.Email.EmailTemplate.txt";
        else
            resourceName = "Majorsilence.Vpn.Logic.Email.EmailTemplate.txt";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream);
        var result = reader.ReadToEnd();
        return result;
    }
}