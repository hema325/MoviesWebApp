using Microsoft.Extensions.Options;
using MoviesWebApplication.Web.WebOptions;
using System.Net;
using System.Net.Mail;

namespace MoviesWebApplication.Web.Services.Email
{
    public class EmailSender:IEmailSender
    {
        private readonly SMTPConfigurationOption smtpConfigurationOption;
        public EmailSender(IOptionsSnapshot<SMTPConfigurationOption> options)
        {
            smtpConfigurationOption = options.Value;
        }

        public async Task SendMailAsync(MailMessage mailMessage)
        {
            mailMessage.From = new MailAddress(smtpConfigurationOption.ServerMail);

            using(var client = new SmtpClient { Port=smtpConfigurationOption.Port,
                Host=smtpConfigurationOption.Host,
                Credentials = new NetworkCredential(smtpConfigurationOption.ServerMail, smtpConfigurationOption.Password),
                EnableSsl=true
            })
            {
                 await client.SendMailAsync(mailMessage);
            }

        }

    }
}
