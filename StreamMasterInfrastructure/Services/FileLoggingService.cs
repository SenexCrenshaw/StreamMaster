using StreamMasterDomain.Services;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.Services;

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
        string formattedMessage = string.Format(format, args);
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
            while (_logQueue.TryDequeue(out string? logEntry))
            {
                await WriteLogEntryAsync(logEntry);
            }

            await Task.Delay(100); // Adjust delay as necessary
        }
    }

    private async Task WriteLogEntryAsync(string logEntry)
    {
        await _writeLock.WaitAsync();
        try
        {
            await File.AppendAllTextAsync(_logFilePath, logEntry + Environment.NewLine);
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

