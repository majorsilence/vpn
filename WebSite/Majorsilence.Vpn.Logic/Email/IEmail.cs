using System;

namespace Majorsilence.Vpn.Logic.Email
{
    public interface IEmail
    {
        void SendMail(string message, string subject, string to, 
                      bool isHtml, byte[] attachment = null, EmailTemplates template = EmailTemplates.None);

        void SendMail_BackgroundThread(string message, string subject, string to, 
                                       bool isHtml, byte[] attachment = null, EmailTemplates template = EmailTemplates.None);

       
    }
}

