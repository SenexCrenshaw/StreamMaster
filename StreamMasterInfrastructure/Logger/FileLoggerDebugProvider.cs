using Microsoft.Extensions.Logging;

using StreamMasterDomain.Services;

namespace StreamMasterInfrastructure.Logger;

public class FileLoggerDebugProvider : ILoggerProvider
{
    private readonly IFileLoggingServiceFactory _factory;

    public FileLoggerDebugProvider(IFileLoggingServiceFactory factory)
    {
        _factory = factory;
    }
    public ILogger CreateLogger(string categoryName)
    {
        return new FileLoggerDebug(_factory);
    }

    public void Dispose()
    {
        // Clean up resources if needed
    }
}
