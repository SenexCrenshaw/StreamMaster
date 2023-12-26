using Microsoft.Extensions.Logging;

namespace StreamMaster.Domain.Logging;
public static class GlobalLoggerProvider
{
    private static ILoggerFactory _loggerFactory;

    public static void Configure(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    public static ILogger CreateLogger(string categoryName)
    {
        return _loggerFactory == null
            ? throw new InvalidOperationException("LoggerFactory is not configured. Call Configure() first.")
            : _loggerFactory.CreateLogger(categoryName);
    }
}
