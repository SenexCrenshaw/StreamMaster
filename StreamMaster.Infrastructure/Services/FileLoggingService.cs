using StreamMaster.Domain.Services;

using System.Collections.Concurrent;

namespace StreamMaster.Infrastructure.Services;

public class FileLoggingService : IFileLoggingService, IDisposable
{
    private readonly ConcurrentQueue<string> _logQueue = new();
    private readonly SemaphoreSlim _writeLock = new(1, 1);
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _loggingTask;
    private readonly string _logFilePath;

    public FileLoggingService(string logFilePath)
    {
        _logFilePath = logFilePath;
        _loggingTask = Task.Run(ProcessLogQueue);
    }

    public void EnqueueLogEntry(string format, params object[] args)
    {
        string formattedMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}" + string.Format(format, args);
        _logQueue.Enqueue(formattedMessage);
    }


    public void EnqueueLogEntry(string logEntry)
    {
        _logQueue.Enqueue(logEntry);
    }

    private async Task ProcessLogQueue()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            while (_writeLock.CurrentCount > 0 && _logQueue.TryDequeue(out string? logEntry))
            {
                await WriteLogEntryAsync(logEntry);
            }

            await Task.Delay(20); // Adjust delay as necessary
        }
    }

    private async Task WriteLogEntryAsync(string logEntry)
    {
        await _writeLock.WaitAsync();
        try
        {
            using FileStream stream = new(_logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            using StreamWriter writer = new(stream);
            await writer.WriteLineAsync(logEntry);
        }
        finally
        {
            _writeLock.Release();
        }
    }

    public async Task StopLoggingAsync()
    {
        _cts.Cancel();
        await _loggingTask;
    }

    public void Dispose()
    {
        _cts.Dispose();
        _writeLock.Dispose();
    }
}

