using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Data;
using DopamineDetoxAPI.Models.Entities;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OpenAI.Chat;
using OpenAI.Images;
using static System.Net.Mime.MediaTypeNames;

namespace DopamineDetoxAPI.Services
{
    public class QuoteService : IQuoteService
    {
        private readonly ChatClient _openAI;
        private readonly ImageClient _imageClient;
        private readonly IMemoryCache _cache;
        private const string QUOTE_CACHE_KEY = "DailyQuote";
        private const string IMAGE_CACHE_KEY = "DailyImage";
        private const string LEARN_MORE_CACHE_KEY = "LearnMore";
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILoggingService _logger;

        public QuoteService(IConfiguration configuration, IMemoryCache cache,
            ApplicationDbContext context, IMapper mapper, ILoggingService logger)
        {
            var apiKey = configuration.GetValue<string>("AppSettings:OPENAI_API_KEY") ?? "";

            _openAI = new("gpt-4", apiKey);
            _imageClient = new("dall-e-3", apiKey);
            _cache = cache;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<(string Quote, byte[] Image)> GetDailyQuoteAndImageAsync(CancellationToken cancellationToken = default)
        {
            // Try to get quote and image from cache
            if (_cache.TryGetValue(QUOTE_CACHE_KEY, out string? cachedQuote) &&
                _cache.TryGetValue(IMAGE_CACHE_KEY, out byte[]? cachedImage) &&
                cachedQuote != null &&
                cachedImage != null)
            {
                return (cachedQuote, cachedImage);
            }

            try
            {
                // Get quote
                var quote = await GetDailyQuoteAsync();

                // Generate image based on quote
                var imagePrompt = $"Create an inspiring and artistic image that represents this quote: '{quote}'. Make it motivational and uplifting.";

                var options = new ImageGenerationOptions
                {
                    Quality = GeneratedImageQuality.Standard,
                    Size = GeneratedImageSize.W1024xH1024,
                    Style = GeneratedImageStyle.Natural,
                    ResponseFormat = GeneratedImageFormat.Bytes
                };

                GeneratedImage generatedImage = _imageClient.GenerateImage(imagePrompt, options);
                byte[] imageBytes = generatedImage.ImageBytes.ToArray();

                // Cache the image for 24 hours
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(24));

                _cache.Set(IMAGE_CACHE_KEY, imageBytes, cacheEntryOptions);

                return (quote, imageBytes);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to generate quote and image", ex);
            }
        }

        public async Task<string> GetDailyQuoteAsync()
        {
            // Try to get quote from cache
            if (_cache.TryGetValue(QUOTE_CACHE_KEY, out string? cachedQuote) && cachedQuote != null)
            {
                return cachedQuote;
            }

            try
            {
                ChatCompletion completion = await _openAI.CompleteChatAsync("Generate a short, powerful motivational quote (maximum 2 sentences) that inspires personal growth and overcoming digital addiction. Make it original and impactful.");

                if (completion?.Content == null || String.IsNullOrEmpty(completion.Content[0].Text))
                {
                    return "No inspirational quote today";
                }

                var quote = completion.Content[0].Text.Trim();

                // Cache the quote for 24 hours
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(24));

                _cache.Set(QUOTE_CACHE_KEY, quote, cacheEntryOptions);

                return quote ?? "No inspirational quote today";
            }
            catch (Exception ex)
            {
                return "No inspirational quote today";
            }
        }

        public async Task<(string introduction, string bulletPoints)> GetLearnMoreDetails(CancellationToken cancellationToken = default)
        {
            if (_cache.TryGetValue(LEARN_MORE_CACHE_KEY, out (string, string)? cached) && cached.HasValue)
            {
                return cached.Value;
            }

            const string prompt = @"
                Provide insights about social media's impact on mental health and use Dr. Andrew Huberman as a frame of reference for how to respond: 

                1. Introduction:
                [Provide 3-5 sentences on the neurological effects of excessive social media use]

                2. Key Impact Points (separate each point with ||):
                [Point 1]||[Point 2]||[Point 3]||[Optional Point 4]||[Optional Point 5]

                Note: Format with clear line break between introduction and impact points. Do not include bullet points or dashes. Please always use '1.Introduction:' before providing the introduction answer and '2.Key Impact Points:' before providing the key impact points so I can properly split each into their own section.";

            try
            {
                ChatCompletion completion = await _openAI.CompleteChatAsync(prompt);
                if (completion?.Content == null || string.IsNullOrEmpty(completion.Content[0].Text))
                {
                    return (string.Empty, string.Empty);
                }

                var response = completion.Content[0].Text.Trim();
                // Extract introduction
                var introStart = response.IndexOf("1. Introduction:") + "1. Introduction:".Length;
                var introEnd = response.IndexOf("2. Key Impact Points:");
                var introduction = response[introStart..introEnd].Trim();

                // Extract bullet points
                var pointsStart = response.IndexOf("2. Key Impact Points:") + "2. Key Impact Points:".Length;
                var bulletPoints = response[pointsStart..].Trim();

                var result = (introduction, bulletPoints);
                _cache.Set(LEARN_MORE_CACHE_KEY, result, TimeSpan.FromHours(24));

                return result;
            }
            catch (Exception e)
            {
                return (string.Empty, string.Empty);
            }
        }

        public async Task<QuoteDto> GetQuoteByDateAsync(DateTime date)
        {
            var quoteEntity = await _context.Quotes
                .FirstOrDefaultAsync(q => q.QuoteDateFetched == date.Date);

            if (quoteEntity == null)
            {
                throw new Exception("No quote found for the specified date");
            }

            var quoteDto = new QuoteDto
            {
                QuoteText = quoteEntity.QuoteText,
                QuoteDateFetched = quoteEntity.QuoteDateFetched.Date,
                QuoteImage = quoteEntity.QuoteImage
            };

            return quoteDto;
        }

        public async Task<Quote?> HasDailyQuoteAsync(CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;
            var quote = await _context.Quotes.Where(q => q.QuoteDateFetched == today).FirstOrDefaultAsync(cancellationToken);
            if (quote == null)
                return null;
            return _mapper.Map<Quote>(quote);
        }

        public async Task<LearnMoreDetail?> HasLearnMoreDetailAsync(CancellationToken cancellationToken = default)
        {
            var today = DateTime.UtcNow.Date;
            var learnMore = await _context.LearnMoreDetails.Where(q => q.QuoteDateFetched == today).FirstOrDefaultAsync(cancellationToken);
            if (learnMore == null)
                return null;
            return _mapper.Map<LearnMoreDetail>(learnMore);
        }

        // In your CreateDailyQuoteAsync method, add token checking:
        public async Task<bool> CreateDailyQuoteAsync(string quote, byte[] image, CancellationToken cancellationToken = default)
        {
            try
            {
                // Add this check
                if (cancellationToken.IsCancellationRequested)
                {
                    await _logger.LogErrorAsync("500","Cancellation was requested before DB operation started",null);
                    return false;
                }

                // Consider adding a timeout
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

                if (String.IsNullOrEmpty(quote))
                {
                    return false;
                }
                if (image == null)
                {
                    image = new byte[0];
                }
                var newQuote = new QuoteEntity
                {
                    QuoteText = quote,
                    QuoteImage = image,
                    QuoteDateFetched = DateTime.UtcNow.Date
                };

                _context.Quotes.Add(newQuote);
                await _context.SaveChangesAsync(linkedCts.Token);
                return true;
            }
            catch (OperationCanceledException ex)
            {
                await _logger.LogErrorAsync("500", "Operation was cancelled during database save", null);
                return false;
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync("500", "Unexpected error during quote creation", null);
                return false;
            }
        }

        public async Task<bool> CreateDailyLearnMoreAsync(string introduction, string bulletPoints, CancellationToken cancellationToken = default)
        {
            try
            {
                // Add this check
                if (cancellationToken.IsCancellationRequested)
                {
                    await _logger.LogErrorAsync("500", "Cancellation was requested before DB operation started", null);
                    return false;
                }

                // Consider adding a timeout
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

                if (String.IsNullOrEmpty(introduction) || String.IsNullOrEmpty(bulletPoints))
                {
                    return false;
                }

                var newLearnMore = new LearnMoreDetailsEntity
                {
                    Introduction = introduction,
                    Items = bulletPoints,
                    QuoteDateFetched = DateTime.UtcNow.Date
                };

                if(newLearnMore != null)
                {
                    _context.LearnMoreDetails.Add(newLearnMore);
                    await _context.SaveChangesAsync(linkedCts.Token);
                    return true;
                }

                return false;

            }
            catch (OperationCanceledException ex)
            {
                await _logger.LogErrorAsync("500", "Operation was cancelled during database save", null);
                return false;
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync("500", "Unexpected error during quote creation", null);
                return false;
            }
        }


    }
}