namespace DopamineDetoxAPI.Models
{
    public class AppSettings
    {
        public string? DopamineDetoxUrl { get; set; }
        public string? Domain { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; } = true;
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? FromEmail { get; set; }
        public string? OPENAI_API_KEY { get; set; }
        public int OpenAIQuoteTimeoutSeconds { get; set; } = 120;
        public int OpenAIImageTimeoutSeconds { get; set; } = 120;
    }
}
