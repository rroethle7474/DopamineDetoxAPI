using DopamineDetoxAPI.Data;
using DopamineDetoxAPI.Models.Entities;
using DopamineDetoxAPI.Services.Interfaces;

public class LoggingService : ILoggingService
{
    private readonly ApplicationDbContext _context;

    public LoggingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogErrorAsync(string? errorCode, string? errorMessage, string? callStack, CancellationToken cancellationToken = default)
    {
        try
        {
            var webTrace = new WebTrace
            {
                DateTimeEncountered = DateTime.UtcNow,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                CallStack = callStack
            };

            _context.WebTraces.Add(webTrace);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch(Exception ex)
        {
            // Log the exception to the console
            Console.WriteLine($"An error occurred while logging an error: {ex.Message}");
        }

    }
}
