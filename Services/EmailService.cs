using DopamineDetoxAPI.Models;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace DopamineDetoxAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _fromEmail;
        private readonly AppSettings _appSettings;

        public EmailService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _smtpClient = new SmtpClient(_appSettings.Domain);
            _smtpClient.UseDefaultCredentials = false;
            _smtpClient.Credentials = new NetworkCredential(_appSettings.Username, _appSettings.Password);
            _smtpClient.Port = _appSettings.Port;
            _smtpClient.EnableSsl = _appSettings.EnableSsl;

            _fromEmail = _appSettings.FromEmail ?? "";
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var mailMessage = new MailMessage(_fromEmail, to, subject, body)
            {
                IsBodyHtml = true // Set to true if the body contains HTML
            };
            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
