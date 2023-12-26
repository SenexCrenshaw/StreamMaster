namespace StreamMaster.Application.Common.Logging;
public static class GlobalLoggerProvider
{
    private static ILoggerFactory _loggerFactory;

    public static void Configure(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    public static ILogger CreateLogger(string categoryName)
    {
        if (_loggerFactory == null)
        {
            throw new InvalidOperationException("LoggerFactory is not configured. Call Configure() first.");
        }

        return _loggerFactory.CreateLogger(categoryName);
    }
}
