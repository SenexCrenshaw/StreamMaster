using Microsoft.Extensions.Logging;

namespace StreamMaster.Infrastructure.Logger;

public class FileLoggerDebugProvider(IFileLoggingServiceFactory factory) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new FileLoggerDebug(factory);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
