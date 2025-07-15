using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MusiKu_API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpHost = _configuration["SmtpSettings:Host"];
            var smtpPort = int.Parse(_configuration["SmtpSettings:Port"]);
            var senderEmail = _configuration["SmtpSettings:SenderEmail"];
            var senderName = _configuration["SmtpSettings:SenderName"];
            var senderPassword = _configuration["SmtpSettings:SenderPassword"];

            var smtpClient = new SmtpClient(smtpHost)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }

}
