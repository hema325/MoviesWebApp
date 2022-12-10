using System.Net.Mail;

namespace MoviesWebApplication.Web.Services.Email
{
    public interface IEmailSender
    {
        Task SendMailAsync(MailMessage mailMessage);
    }
}
