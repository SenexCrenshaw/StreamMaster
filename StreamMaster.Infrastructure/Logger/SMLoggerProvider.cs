using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Services;

namespace StreamMaster.Infrastructure.Logger;

public class SMLoggerProvider : ILoggerProvider
{
    private readonly IFileLoggingServiceFactory _factory;

    public SMLoggerProvider(IFileLoggingServiceFactory factory)
    {
        _factory = factory;
    }
    public ILogger CreateLogger(string categoryName)
    {
        return new SMLogger(_factory);
    }

    public void Dispose()
    {
        // Clean up resources if needed
    }
}
