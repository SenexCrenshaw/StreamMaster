using Microsoft.Extensions.Logging;

namespace StreamMasterInfrastructure.Logging;

public class SMLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new SMLogger();
    }

    public void Dispose()
    {
        // Clean up resources if needed
    }
}
