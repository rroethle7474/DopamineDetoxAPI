using DopamineDetox.Domain.Models;

namespace DopamineDetoxAPI.Services.Interfaces
{
    public interface IQuoteService
    {
        Task<(string Quote, byte[] Image)> GetDailyQuoteAndImageAsync(CancellationToken cancellationToken = default);
        Task<Quote?> HasDailyQuoteAsync(CancellationToken cancellationToken = default);
        Task<bool> CreateDailyQuoteAsync(string quote, byte[] image, CancellationToken cancellationToken = default);
        Task<(string introduction, string bulletPoints)> GetLearnMoreDetails(CancellationToken cancellationToken = default);
        Task<LearnMoreDetail?> HasLearnMoreDetailAsync(CancellationToken cancellationToken = default);
        Task<bool> CreateDailyLearnMoreAsync(string introduction, string bulletPoints, CancellationToken cancellationToken = default);
    }
}
