using System;
using System.IO;

namespace LibLogic.Email
{
    public class FakeEmail : IEmail
    {
        public FakeEmail()
        {
        }

        public void SendMail(string message, string subject, string to, 
            bool isHtml, byte[] attachment=null, EmailTemplates template = EmailTemplates.None)
        {


            if (attachment != null)
            {
                var attachData = new System.Net.Mail.Attachment(new MemoryStream(attachment), "");
            }

            if (message.Trim() == "" || subject.Trim() == "" || to.Trim() == "")
            {
                throw new System.Net.Mail.SmtpException("Invalid data passed into the FakeEmail class");
            }
 
      
        }

        public void SendMail_BackgroundThread(string message, string subject, string to, bool isHtml, 
            byte[] attachment = null, EmailTemplates template = EmailTemplates.None)
        {

           
            var caller = new Action<string, string, string, bool, byte[], EmailTemplates>(SendMail);
            caller.BeginInvoke(message, subject, to, isHtml, attachment, template, null, null);

        }

    }
}

