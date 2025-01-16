using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DopamineDetoxAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuoteController : Controller
    {
        private readonly ILogger<QuoteController> _logger;
        private readonly IQuoteService _quoteService;

        public QuoteController(ILogger<QuoteController> logger, IQuoteService quoteService)
        {
            _logger = logger;
            _quoteService = quoteService;
        }

        [HttpGet("daily")]
        public async Task<ActionResult<QuoteDto>> GetDailyQuote(CancellationToken cancellationToken)
        {
            var defaultQuote = new QuoteDto
            {
                QuoteText = "You are the master of your destiny. You can influence, direct and control your own environment. You can make your life what you want it to be.",
                QuoteDateFetched = DateTime.UtcNow,
            };

            try 
            {
                var dailyQuote = await _quoteService.HasDailyQuoteAsync(cancellationToken);
                if (!String.IsNullOrEmpty(dailyQuote?.QuoteText))
                {
                    return Ok(new QuoteDto
                    {
                        QuoteText = dailyQuote.QuoteText,
                        QuoteDateFetched = dailyQuote.QuoteDateFetched,
                        QuoteImage = dailyQuote.QuoteImage
                    });
                }

                return Ok(defaultQuote);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error generating quote");
                return Ok(defaultQuote);
            }
        }

        [HttpGet("learn-more")]
        public async Task<ActionResult<LearnMoreDetailDto>> GetLearnMoreDetails(CancellationToken cancellationToken)
        {
            var defaultLearnMore = new LearnMoreDetailDto
            {
                Introduction = "From a neurobiological perspective, excessive social media use can dramatically alter our brain's reward circuitry and dopamine systems. The constant stream of notifications and variable reward schedules creates powerful intermittent reinforcement patterns, similar to those observed in addictive behaviors. This digital overwhelm can suppress activity in the prefrontal cortex, the region responsible for executive function and impulse control, while simultaneously elevating cortisol levels and disrupting our natural circadian rhythms.",
                Items = "Chronic social media use impairs deep work and focus, reducing attention span to as little as 8 seconds in frequent users||The constant context-switching and digital multitasking can increase anxiety levels and decrease gray matter density in regions associated with emotional regulation||Regular exposure to curated highlight reels of others' lives triggers social comparison and can lead to decreased self-esteem and increased depressive symptoms||The blue light emission from devices during nighttime social media use disrupts melatonin production, leading to poor sleep quality and compromised cognitive performance",
            QuoteDateFetched = DateTime.UtcNow,
            };

            try
            {
                var learnMore = await _quoteService.HasLearnMoreDetailAsync(cancellationToken);
                if (!String.IsNullOrEmpty(learnMore?.Introduction))
                {
                    return Ok(new LearnMoreDetailDto
                    {
                        Introduction = learnMore.Introduction,
                        QuoteDateFetched = learnMore.QuoteDateFetched,
                        Items = learnMore.Items
                    });
                }

                return Ok(defaultLearnMore);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving learn more");
                return Ok(defaultLearnMore);
            }
        }

        [HttpGet("create-daily")]
        public async Task<ActionResult<bool>> CreateDailyQuote(CancellationToken cancellationToken)
        {
            try
            {
                var dailyQuote = await _quoteService.HasDailyQuoteAsync(cancellationToken);

                if (!String.IsNullOrEmpty(dailyQuote?.QuoteText))
                {
                    //_logger.LogError(ex, "Daily Quote already generated for today");
                    return false;
                }

                var (Quote, Image) = await _quoteService.GetDailyQuoteAndImageAsync(cancellationToken);
                var created = await _quoteService.CreateDailyQuoteAsync(Quote, Image, cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating quote");
               return false;
            }
        }

        [HttpGet("create-learn-more")]
        public async Task<ActionResult<bool>> CreateLearnMore(CancellationToken cancellationToken)
        {
            try
            {
                var learnMore = await _quoteService.HasLearnMoreDetailAsync(cancellationToken);

                if (!String.IsNullOrEmpty(learnMore?.Introduction))
                {
                    //_logger.LogError(ex, "Daily Quote already generated for today");
                    return false;
                }

                var (introduction, bulletPoints) = await _quoteService.GetLearnMoreDetails(cancellationToken);
                var created = await _quoteService.CreateDailyLearnMoreAsync(introduction, bulletPoints, cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating quote");
                return false;
            }
        }
    }
}