using System;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Majorsilence.Vpn.Logic.Email;

public class FakeEmail : IEmail
{
    public Task SendMail(string message, string subject, string to,
        bool isHtml, byte[] attachment = null, EmailTemplates template = EmailTemplates.None)
    {
        if (attachment != null)
        {
            var attachData = new Attachment(new MemoryStream(attachment), "");
        }

        if (message.Trim() == "" || subject.Trim() == "" || to.Trim() == "")
            throw new SmtpException("Invalid data passed into the FakeEmail class");

        return Task.CompletedTask;
    }
}