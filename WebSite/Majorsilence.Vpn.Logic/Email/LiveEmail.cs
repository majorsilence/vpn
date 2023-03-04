using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
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

    public async Task SendMail(string message, string subject, string to,
        bool isHtml, byte[] attachment = null, EmailTemplates template = EmailTemplates.None)
    {
        using var mm = new MailMessage();
        using var smcl = new SmtpClient();
        mm.To.Add(to);
        
        mm.From = new MailAddress(fromAddress);
        mm.IsBodyHtml = isHtml;
        mm.Body = message;
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
        
        await smcl.SendMailAsync(mm);
    }
}