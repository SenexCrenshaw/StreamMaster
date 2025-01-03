using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Extensions;

namespace StreamMaster.Infrastructure.Logger;

public class SMLogger(IFileLoggingServiceFactory factory) : ILogger
{
    private readonly IFileLoggingService _logging = factory.Create("SMLogger");

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        string logEntry = FormatLogEntry(logLevel, eventId, state, exception, formatter);
        _logging.EnqueueLogEntry(logEntry);
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    //public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    //{
    //    if (!IsEnabled(logLevel))
    //    {
    //        return;
    //    }

    //    // Format the log Message
    //    string Message = formatter(state, exception);

    //    // Save the log entry to the SQLite database
    //    using LogDbContext db = new();
    //    db.LogEntries.GetOrAdd(new LogEntry
    //    {
    //        LogLevel = logLevel,
    //        MessageId = Message,
    //        TimeStamp = DateTime.Now
    //    });

    //    // If there's an exception, log its details separately
    //    if (exception != null)
    //    {
    //        db.LogEntries.GetOrAdd(new LogEntry
    //        {
    //            LogLevel = LogLevel.Error,
    //            MessageId = exception.ToString(),
    //            TimeStamp = DateTime.Now
    //        });
    //    }

    //    db.SaveChanges();
    //}

    //public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    //{
    //    if (!IsEnabled(logLevel))
    //    {
    //        return;
    //    }

    //    string logEntry = FormatLogEntry(logLevel, eventId, state, exception, formatter);
    //    _logQueue.Enqueue(logEntry);
    //}

    private static string FormatLogEntry<TState>(LogLevel logLevel, EventId eventId, TState? state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (state == null)
        {
            return string.Empty;
        }

        string message = formatter(state, exception);

        // Format the log entry as CSV, including the EventId
        string csvFormattedEntry = $"\"{SMDT.UtcNow:yyyy-MM-dd HH:mm:ss}\",\"{logLevel}\",\"{eventId.Id}\",\"{eventId.Name}\",\"{message.Replace("\"", "\"\"")}\"";
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

    //private async Task WriteLogEntryAsync(string logEntry)
    //{
    //    await _writeLock.WaitAsync();
    //    try
    //    {
    //        await File.AppendAllTextAsync(BuildInfo.LogFilePath, logEntry + Environment.NewLine);
    //    }
    //    finally
    //    {
    //        _writeLock.Release();
    //    }
    //}
}