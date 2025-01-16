using Azure.Communication.Email;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Models.Entities;
using DopamineDetoxAPI.Services.Interfaces;

public class CloudEmailService : ICloudEmailService
{
    private readonly EmailClient _emailClient;
    private readonly string _senderAddress;
    private readonly IEmailTemplateService _emailTemplateService;

    public CloudEmailService(string connectionString, string senderAddress, IEmailTemplateService emailTemplateService)
    {
        _emailClient = new EmailClient(connectionString);
        _senderAddress = senderAddress;
        _emailTemplateService = emailTemplateService;
    }

    public async Task<bool> SendMVPWeeklyEmail(User user, List<SearchResult> results, CancellationToken cancellationToken = default)
    {
        
        if(String.IsNullOrEmpty(user?.Email))
        {
            throw new Exception("No email address provided.");
        }

        var (emailBody, subject) = await _emailTemplateService.BuildUserMVPWeeklyEmail(user, results, cancellationToken);

        if(String.IsNullOrEmpty(emailBody) || String.IsNullOrEmpty(subject))
        {
            throw new Exception("No valid HtmlContent returned");
        }

        var emailContent = new EmailContent(subject)
        {
            Html = emailBody
        };

        var emailMessage = new EmailMessage(
            senderAddress: _senderAddress,
            recipientAddress: user.Email,
            content: emailContent);

        try
        {
            var response = await _emailClient.SendAsync(Azure.WaitUntil.Completed, emailMessage, cancellationToken);
            var messageId = response.Value.ToString();
        }
        catch (Exception ex)
        {
            // Handle error appropriately
            throw new Exception(ex.ToString());
        }

        return true;
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

        var emailContent = new EmailContent(subject)
        {
            Html = emailBody
        };

        var emailMessage = new EmailMessage(
            senderAddress: _senderAddress,
            recipientAddress: user.Email,
            content: emailContent);

        try
        {
            var response = await _emailClient.SendAsync(Azure.WaitUntil.Completed, emailMessage, cancellationToken);
            var messageId = response.Value.ToString();
        }
        catch (Exception ex)
        {
            // Handle error appropriately
            throw new Exception(ex.ToString());
        }
        return true;
    }
}