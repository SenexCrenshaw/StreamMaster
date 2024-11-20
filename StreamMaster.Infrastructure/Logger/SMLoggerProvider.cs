using Microsoft.Extensions.Logging;

namespace StreamMaster.Infrastructure.Logger;

public class SMLoggerProvider : ILoggerProvider
{
    private readonly IFileLoggingServiceFactory _factory;
    private readonly SMLogger _sharedLogger; // Shared instance of SMLogger

    public SMLoggerProvider(IFileLoggingServiceFactory factory)
    {
        _factory = factory;
        _sharedLogger = new SMLogger(_factory); // Create a single SMLogger instance
    }

    public ILogger CreateLogger(string categoryName)
    {
        // Reuse the same SMLogger instance for all categories
        return _sharedLogger;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}