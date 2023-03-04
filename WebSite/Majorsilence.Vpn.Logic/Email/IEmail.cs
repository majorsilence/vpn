using System.Threading.Tasks;

namespace Majorsilence.Vpn.Logic.Email;

public interface IEmail
{
    Task SendMail(string message, string subject, string to,
        bool isHtml, byte[] attachment = null, EmailTemplates template = EmailTemplates.None);
}