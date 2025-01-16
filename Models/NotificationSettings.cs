namespace DopamineDetoxAPI.Models
{
    public class NotificationSettings
    {
        public string SendGridApiKey { get; set; } = string.Empty;
        public string SendGridFromEmail { get; set; } = string.Empty;
        public string SendGridFromName { get; set; } = string.Empty;
    }
}
