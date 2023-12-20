using Microsoft.Extensions.Logging;

namespace StreamMasterInfrastructure.Logging;

public class SMLogger : ILogger
{
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

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        // Format the log message
        string message = formatter(state, exception);

        // Save the log entry to the SQLite database
        using LogDbContext db = new();
        db.LogEntries.Add(new LogEntry
        {
            LogLevel = logLevel,
            Message = message,
            TimeStamp = DateTime.Now
        });

        // If there's an exception, log its details separately
        if (exception != null)
        {
            db.LogEntries.Add(new LogEntry
            {
                LogLevel = LogLevel.Error,
                Message = exception.ToString(),
                TimeStamp = DateTime.Now
            });
        }

        db.SaveChanges();
    }
}
