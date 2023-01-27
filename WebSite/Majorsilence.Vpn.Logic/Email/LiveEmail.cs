using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using Majorsilence.Vpn.Logic.Helpers;

namespace Majorsilence.Vpn.Logic.Email;

public class LiveEmail : IEmail
{
    private readonly string fromAddress;
    private readonly string host;
    private readonly string password;
    private readonly int port;
    private readonly string username;

    public LiveEmail(string fromAddress, string username, string password, string host, int port)
    {
        this.fromAddress = fromAddress;
        this.username = username;
        this.password = password;
        this.host = host;
        this.port = port;
    }

    public void SendMail(string message, string subject, string to,
        bool isHtml, byte[] attachment = null, EmailTemplates template = EmailTemplates.None)
    {
        var mm = new MailMessage();
        var smcl = new SmtpClient();

        mm.To.Add(to);

        var sendMessage = message;
        if (template != EmailTemplates.None) sendMessage = ProcessMessage(template, message, subject);

        mm.From = new MailAddress(fromAddress);
        mm.IsBodyHtml = isHtml;
        mm.Body = sendMessage;
        mm.Subject = subject;
        if (attachment != null)
        {
            var attachData = new Attachment(new MemoryStream(attachment), "");
            mm.Attachments.Add(attachData);
        }

        smcl.Host = host;
        smcl.Port = port;
        smcl.Credentials = new NetworkCredential(username, password);
        smcl.EnableSsl = true;

        // This line is very bad.  It will always say the smtp server ssl is valid even if it is not. 
        // This is required because mono on linux does not have gmail ssl certs and there is no way to auto import
        // them using vagrant.  See bootstrap file "bootstrap-website.sh" function "configure_gmail_ssl_certs_for_mono"
        // or
        // mozroots --import --ask-remove --machine
        // certmgr -ssl smtps://smtp.gmail.com:465
        // certmgr -ssl smtps://smtp.gmail.com:587
        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

        smcl.Send(mm);
    }

    public void SendMail_BackgroundThread(string message, string subject, string to, bool isHtml,
        byte[] attachment = null, EmailTemplates template = EmailTemplates.None)
    {
        // TODO: stick it in a background queue task
        var caller = new Action<string, string, string, bool, byte[], EmailTemplates>(SendMail);
        caller.BeginInvoke(message, subject, to, isHtml, attachment, template, null, null);
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


        using (var stream = assembly.GetManifestResourceStream(resourceName))
        using (var reader = new StreamReader(stream))
        {
            var result = reader.ReadToEnd();
            return result;
        }
    }
}