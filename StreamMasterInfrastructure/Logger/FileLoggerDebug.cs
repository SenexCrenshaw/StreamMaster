using Microsoft.Extensions.Logging;

using StreamMasterDomain.Services;

namespace StreamMasterInfrastructure.Logger;

public class FileLoggerDebug : ILogger
{
    private readonly IFileLoggingService _logging;
    public FileLoggerDebug(IFileLoggingServiceFactory factory)
    {
        _logging = factory.Create("FileLoggerDebug");
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        string logEntry = FormatLogEntry(logLevel, eventId, state, exception, formatter);
        _logging.EnqueueLogEntry(logEntry);
    }


    public IDisposable BeginScope<TState>(TState state)
    {
        return null; // If you have a scope to handle, return an object that implements IDisposable
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        // Determine if the log level should be enabled or not (you can set your
        // criteria here)
        return true;
    }
    private static string FormatLogEntry<TState>(LogLevel logLevel, EventId eventId, TState? state, Exception exception, Func<TState, Exception, string> formatter)
    {

        string message = formatter(state, exception);

        // Format the log entry as CSV, including the EventId
        string csvFormattedEntry = $"\"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\",\"{logLevel}\",\"{eventId.Id}\",\"{eventId.Name}\",\"{message.Replace("\"", "\"\"")}\"";
        if (exception != null)
        {
            csvFormattedEntry += $",\"{exception.ToString().Replace("\"", "\"\"")}\"";
        }
        else
        {
            csvFormattedEntry += ",";
        }

        return csvFormattedEntry;
    }


}
