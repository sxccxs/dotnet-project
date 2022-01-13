using System.Net;
using System.Net.Mail;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.Settings;
using Microsoft.Extensions.Options;

namespace BLL.Services.UserServices
{
    internal class EmailService : IEmailService
    {
        private readonly EmailSettings emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            this.emailSettings = emailSettings?.Value ?? throw new ArgumentNullException(nameof(emailSettings));
        }

        public void SendEmail(string toEmail, string emailSubject, string emailHtmlBody)
        {
            var message = new MailMessage
            {
                From = new MailAddress(this.emailSettings.EmailAccountLogin),
                Subject = emailSubject,
                IsBodyHtml = true,
                Body = emailHtmlBody,
            };
            message.To.Add(new MailAddress(toEmail));

            var smtp = new SmtpClient
            {
                Port = this.emailSettings.EmailPort,
                Host = this.emailSettings.EmailHost,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(this.emailSettings.EmailAccountLogin, this.emailSettings.EmailAccountPassword),
                DeliveryMethod = SmtpDeliveryMethod.Network,
            };

            smtp.Send(message);
        }
    }
}
