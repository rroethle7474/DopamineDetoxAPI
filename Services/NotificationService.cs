using Azure.Communication.Email;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Models;
using DopamineDetoxAPI.Models.Entities;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace DopamineDetoxAPI.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationSettings _settings;
        private readonly ILogger<NotificationService> _logger;
        private readonly SendGridClient _sendGridClient;
        private readonly IEmailTemplateService _emailTemplateService;

        public NotificationService(
            IOptions<NotificationSettings> settings,
            ILogger<NotificationService> logger,
            IEmailTemplateService emailTemplateService)
        {
            _settings = settings.Value;
            _logger = logger;
            _sendGridClient = new SendGridClient(_settings.SendGridApiKey);
            _emailTemplateService = emailTemplateService;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlContent)
        {
            try
            {
                var msg = CreateEmailMessage(new[] { to }, subject, htmlContent);
                var response = await _sendGridClient.SendEmailAsync(msg);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to send email to {To}. Status code: {StatusCode}",
                        to, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {To}", to);
                throw;
            }
        }

        public async Task SendEmailsAsync(IEnumerable<string> to, string subject, string htmlContent)
        {
            try
            {
                var msg = CreateEmailMessage(to, subject, htmlContent);
                var response = await _sendGridClient.SendEmailAsync(msg);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to send emails. Status code: {StatusCode}",
                        response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending multiple emails");
                throw;
            }
        }

        private SendGridMessage CreateEmailMessage(IEnumerable<string> to, string subject, string htmlContent)
        {
            var msg = new SendGridMessage
            {
                From = new SendGrid.Helpers.Mail.EmailAddress(_settings.SendGridFromEmail, _settings.SendGridFromName),
                Subject = subject,
                HtmlContent = htmlContent
            };

            msg.AddTos(to.Select(email => new SendGrid.Helpers.Mail.EmailAddress(email)).ToList());
            return msg;
        }

        public async Task<bool> SendPasswordResetEmail(ApplicationUser user, string encodedToken, string baseUrl, CancellationToken cancellationToken = default)
        {
            if (String.IsNullOrEmpty(user?.Email))
            {
                throw new Exception("No email address provided.");
            }

            var (emailBody, subject) = await _emailTemplateService.BuildResetPasswordEmail(user, encodedToken, baseUrl, cancellationToken);

            if (String.IsNullOrEmpty(emailBody) || String.IsNullOrEmpty(subject))
            {
                throw new Exception("No valid HtmlContent returned");
            }

            try
            {
                await SendEmailAsync(user.Email, subject, emailBody);
            }
            catch (Exception ex)
            {
                // Handle error appropriately
                throw new Exception(ex.ToString());
            }
            return true;
        }

        public async Task<bool> SendMVPWeeklyEmail(User user, List<SearchResult> results, CancellationToken cancellationToken = default)
        {

            if (String.IsNullOrEmpty(user?.Email))
            {
                throw new Exception("No email address provided.");
            }

            var (emailBody, subject) = await _emailTemplateService.BuildUserMVPWeeklyEmail(user, results, cancellationToken);

            if (String.IsNullOrEmpty(emailBody) || String.IsNullOrEmpty(subject))
            {
                throw new Exception("No valid HtmlContent returned");
            }

            var emailContent = new EmailContent(subject)
            {
                Html = emailBody
            };

            try
            {
                await SendEmailAsync(user.Email, subject, emailBody);
            }
            catch (Exception ex)
            {
                // Handle error appropriately
                throw new Exception(ex.ToString());
            }

            return true;
        }
    }
}
