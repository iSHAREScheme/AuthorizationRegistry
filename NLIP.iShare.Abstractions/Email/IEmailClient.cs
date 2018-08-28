using System.Threading.Tasks;

namespace NLIP.iShare.Abstractions.Email
{
    /// <summary>
    /// Send an email to a recipient
    /// </summary>
    public interface IEmailClient
    {
        Task Send(string to, string subject, string body);
        Task Send(string from, string to, string subject, string body);
        Task Send(EmailAddress to, string subject, string body);
        Task Send(EmailAddress from, EmailAddress to, string subject, string body);
    }
}
