using Microsoft.Extensions.Logging;

using StreamMasterDomain.Common;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.Logging;

public class SMLogger : ILogger
{
    private readonly ConcurrentQueue<string> _logQueue = new();
    private readonly SemaphoreSlim _writeLock = new(1, 1);
    private readonly CancellationTokenSource _cts = new();

    private readonly Task _loggingTask;
    private readonly string LogFilePath = "";
    public SMLogger()
    {
        _loggingTask = Task.Run(ProcessLogQueue);
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        LogFilePath = Path.Combine(BuildInfo.AppDataFolder, $"StreamMasterAPI_{timestamp}.log");
    }

    private async Task ProcessLogQueue()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            while (_logQueue.TryDequeue(out string? logEntry))
            {
                await WriteLogEntryAsync(logEntry);
            }

            await Task.Delay(100); // Adjust delay as necessary
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        try
        {
            _loggingTask.Wait();
        }
        catch (AggregateException)
        {
            // Handle exceptions or ignore
        }
        _cts.Dispose();
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

    //public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    //{
    //    if (!IsEnabled(logLevel))
    //    {
    //        return;
    //    }

    //    // Format the log message
    //    string message = formatter(state, exception);

    //    // Save the log entry to the SQLite database
    //    using LogDbContext db = new();
    //    db.LogEntries.Add(new LogEntry
    //    {
    //        LogLevel = logLevel,
    //        Message = message,
    //        TimeStamp = DateTime.Now
    //    });

    //    // If there's an exception, log its details separately
    //    if (exception != null)
    //    {
    //        db.LogEntries.Add(new LogEntry
    //        {
    //            LogLevel = LogLevel.Error,
    //            Message = exception.ToString(),
    //            TimeStamp = DateTime.Now
    //        });
    //    }

    //    db.SaveChanges();
    //}



    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        string logEntry = FormatLogEntry(logLevel, eventId, state, exception, formatter);
        _logQueue.Enqueue(logEntry);
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


    private async Task WriteLogEntryAsync(string logEntry)
    {
        await _writeLock.WaitAsync();
        try
        {
            await File.AppendAllTextAsync(LogFilePath, logEntry + Environment.NewLine);
        }
        finally
        {
            _writeLock.Release();
        }
    }

}
