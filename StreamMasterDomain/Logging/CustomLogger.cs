using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Cache;

using System.Text.RegularExpressions;

namespace StreamMasterDomain.Logging;
public class CustomLogger<T>(ILoggerFactory loggerFactory, ILoggingUtils loggingUtils, IMemoryCache memoryCache) : ILogger<T>
{
    private readonly ILogger _innerLogger = loggerFactory.CreateLogger<T>();
    private readonly ILoggingUtils _loggingUtils = loggingUtils ?? throw new ArgumentNullException(nameof(loggingUtils));

    public IDisposable BeginScope<TState>(TState state)
    {
        return _innerLogger.BeginScope(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _innerLogger.IsEnabled(logLevel);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        Setting setting = memoryCache.GetSetting();
        if (!setting.CleanURLs)
        {
            _innerLogger.Log(logLevel, eventId, state, exception, formatter);
            return;
        }

        string originalMessage = formatter(state, exception);

        // Modify the message as needed, for example replace the streamUrl with loggableUrl
        string modifiedMessage = ReplaceStreamUrl(originalMessage).Result;

        _innerLogger.Log(logLevel, eventId, state, exception, (s, e) => modifiedMessage);
    }


    private string ExtractStreamUrl(string originalMessage)
    {
        // Regular expression to match URLs
        Regex regex = new(@"https?://\S+");
        Match match = regex.Match(originalMessage);

        return match.Success ? match.Value : string.Empty;
    }

    private async Task<string> ReplaceStreamUrl(string originalMessage)
    {
        string streamUrl = ExtractStreamUrl(originalMessage);

        if (string.IsNullOrEmpty(streamUrl))
        {
            // If no streamUrl is found, just return the original message
            return originalMessage;
        }

        string loggableUrl = await _loggingUtils.GetLoggableURLAsync(streamUrl);

        return originalMessage.Replace(streamUrl, loggableUrl);
    }


}
