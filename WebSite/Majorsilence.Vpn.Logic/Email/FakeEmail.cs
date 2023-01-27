using System;
using System.IO;
using System.Net.Mail;

namespace Majorsilence.Vpn.Logic.Email;

public class FakeEmail : IEmail
{
    public void SendMail(string message, string subject, string to,
        bool isHtml, byte[] attachment = null, EmailTemplates template = EmailTemplates.None)
    {
        if (attachment != null)
        {
            var attachData = new Attachment(new MemoryStream(attachment), "");
        }

        if (message.Trim() == "" || subject.Trim() == "" || to.Trim() == "")
            throw new SmtpException("Invalid data passed into the FakeEmail class");
    }

    public void SendMail_BackgroundThread(string message, string subject, string to, bool isHtml,
        byte[] attachment = null, EmailTemplates template = EmailTemplates.None)
    {
        var caller = new Action<string, string, string, bool, byte[], EmailTemplates>(SendMail);
        caller.BeginInvoke(message, subject, to, isHtml, attachment, template, null, null);
    }
}