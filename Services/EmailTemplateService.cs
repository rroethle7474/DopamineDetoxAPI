using DopamineDetox.Domain.Enums;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Data;
using DopamineDetoxAPI.Models;
using DopamineDetoxAPI.Models.Entities;
using DopamineDetoxAPI.Services;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;

public class EmailTemplateService : IEmailTemplateService
{
    private readonly ApplicationDbContext _context;
    private readonly CacheService _cacheService;

    // add new logging service to contructor


    public EmailTemplateService(ApplicationDbContext context, CacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<(string htmlBodyContent, string subject)> BuildUserMVPWeeklyEmail(User user, List<SearchResult> articles, CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder();

        if(String.IsNullOrEmpty(user?.Email))
        {
            throw new Exception("No email address provided.");
        }

        var weeklyReportTemplate = await _context.EmailTemplates.
            FirstOrDefaultAsync(et => et.Id == (int)EmailTemplateType.WeeklyMVPReport, cancellationToken);

        if (weeklyReportTemplate == null)
        {
            throw new Exception("No template");
        }

        var reportSubject = weeklyReportTemplate.Subject;

        sb.Append(weeklyReportTemplate.HtmlBodyContent);

        var headerTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(et => et.Id == (int)EmailTemplateType.Header, cancellationToken);
        var footerTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(et => et.Id == (int)EmailTemplateType.Footer, cancellationToken);

        if(!String.IsNullOrEmpty(headerTemplate?.HtmlBodyContent))
        {
            var headerBody = headerTemplate.HtmlBodyContent; 
            headerBody = headerBody
                .Replace("@name", user.UserName)
                .Replace("@date", DateTime.Now.ToString("d"));
            sb.Replace("@header", headerBody);
        }

        if (!String.IsNullOrEmpty(footerTemplate?.HtmlBodyContent))
        {
            var footerBody = footerTemplate.HtmlBodyContent;
            sb.Replace("@footer", footerBody);
        }

        var searchResultTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(et => et.Id == (int)EmailTemplateType.WeeklySearchResult, cancellationToken);
        var notesTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(et => et.Id == (int)EmailTemplateType.NotesSection, cancellationToken);
        var noteItemTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(et => et.Id == (int)EmailTemplateType.NoteItem, cancellationToken);

        if(String.IsNullOrEmpty(searchResultTemplate?.HtmlBodyContent) ||
            String.IsNullOrEmpty(notesTemplate?.HtmlBodyContent) ||
            String.IsNullOrEmpty(noteItemTemplate?.HtmlBodyContent))
        {
            throw new Exception("Please review email templates as not all valid ones have been found");
        }

        foreach (var article in articles)
        {
            var searchResult = searchResultTemplate.HtmlBodyContent;
            searchResult = searchResult
                .Replace("@title", article.Title)
                .Replace("@url", article.Url);

            var notesSection = notesTemplate.HtmlBodyContent;

            if (article.Notes != null && article.Notes.Count() > 0)
            {
                var noteSectionItems = new StringBuilder();
                foreach (var note in article.Notes)
                {
                    var notesItem = noteItemTemplate.HtmlBodyContent;
                    notesItem = notesItem
                        .Replace("@notetitle", note.Title)
                        .Replace("@notemessage", note.Message);
                    noteSectionItems.Append(notesItem);

                }
                notesSection.Replace("notessection", noteSectionItems.ToString());
            }
            else
            {
                searchResult.Replace("@notessection", "");
            }

            sb.Append(searchResult);
        }
        string htmlBodyContent = sb.ToString();
        return (htmlBodyContent, reportSubject);
    }

    public async Task<(string htmlBodyContent, string subject)> BuildResetPasswordEmail(ApplicationUser user, string encodedToken, string  baseUrl, CancellationToken cancellationToken = default)
    {
        if (String.IsNullOrEmpty(user?.Email))
        {
            throw new Exception("No email address provided.");
        }

        if (String.IsNullOrEmpty(encodedToken))
        {
            throw new Exception("No token provided");
        }

        var weeklyReportTemplate = await _context.EmailTemplates.
            FirstOrDefaultAsync(et => et.Id == (int)EmailTemplateType.ResetPassword, cancellationToken);

        if (weeklyReportTemplate == null)
        {
            throw new Exception("No template");
        }

        var reportSubject = weeklyReportTemplate.Subject;

        var encodedEmail = WebUtility.UrlEncode(user.Email);

        // Assuming your Angular app is served from the same domain
        var resetLink = $"{baseUrl}/reset-password?token={encodedToken}&email={encodedEmail}";

        if (!String.IsNullOrEmpty(weeklyReportTemplate?.HtmlBodyContent))
        {
            var bodyText = weeklyReportTemplate.HtmlBodyContent;

            var greetingFirstName = String.Empty;
            var greetingLastName = String.Empty;

            if(String.IsNullOrEmpty(user?.FirstName))
            {
                greetingFirstName = user?.UserName;
            }
            else
            {
                greetingFirstName = user?.FirstName;
                greetingLastName = user?.LastName;
            }

            bodyText = bodyText
                .Replace("@firstName", greetingFirstName)
                .Replace("@lastName", greetingLastName)
                .Replace("@resetLink", resetLink);
            return (bodyText, reportSubject);
        }
        throw new Exception("No valid HtmlContent returned");
    }
}