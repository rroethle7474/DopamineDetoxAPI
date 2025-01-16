namespace DopamineDetoxAPI.Services.Interfaces
{
    public interface ILoggingService
    {
        Task LogErrorAsync(string? errorCode, string? errorMessage, string? callStack, CancellationToken cancellationToken = default);
    }
}
