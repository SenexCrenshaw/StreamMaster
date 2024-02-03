using StreamMaster.Domain.Common;
using StreamMaster.Domain.Services;

using System.Collections.Concurrent;
using System.Text;

namespace StreamMaster.Infrastructure.Services;

public class FileLoggingService : IFileLoggingService, IDisposable
{
    private readonly ConcurrentQueue<string> _logQueue = new();
    private static readonly SemaphoreSlim _writeLock = new(1, 1);
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
        string formattedMessage = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}" + string.Format(format, args);
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
            StringBuilder combinedLogEntries = new();
            await _writeLock.WaitAsync();
            try
            {
                while (_logQueue.TryDequeue(out string? logEntry))
                {
                    if (logEntry != null)
                    {
                        combinedLogEntries.AppendLine(logEntry);
                    }
                }

                if (combinedLogEntries.Length > 0)
                {
                    await WriteLogEntryAsync(combinedLogEntries.ToString());
                }
            }
            finally
            {
                _writeLock.Release();
            }

            await Task.Delay(100); // Adjust as necessary
        }
    }

    private async Task WriteLogEntryAsync(string logEntry)
    {
        try
        {
            RotateLogIfNeeded();

            using FileStream stream = new(_logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            using StreamWriter writer = new(stream);
            await writer.WriteLineAsync(logEntry);
        }
        catch (Exception ex)
        {
            //Exception a = ex;
        }

    }

    private void RotateLogIfNeeded()
    {
        Setting setting = FileUtil.GetSetting();
        FileInfo logFileInfo = new(_logFilePath);
        long maxFileSizeInBytes = Math.Max(1 * 1024 * 1024, Math.Min(setting.MaxLogFileSizeMB * 1024 * 1024, 100 * 1024 * 1024)); // Convert MB to Bytes

        if (logFileInfo.Exists && logFileInfo.Length > maxFileSizeInBytes)
        {
            string directory = logFileInfo.DirectoryName;
            string baseFileName = Path.GetFileNameWithoutExtension(logFileInfo.FullName);
            string extension = logFileInfo.Extension;

            // Bump log files, renaming log.N.log to log.(N+1).log
            for (int i = setting.MaxLogFiles - 1; i >= 1; i--)
            {
                string oldFileName = Path.Combine(directory, $"{baseFileName}.{i}{extension}");
                string newFileName = Path.Combine(directory, $"{baseFileName}.{i + 1}{extension}");

                if (File.Exists(newFileName))
                {
                    File.Delete(newFileName);
                }

                if (File.Exists(oldFileName))
                {
                    File.Move(oldFileName, newFileName);
                }
            }

            // Rename the current log to .1.log
            string rotatedFileName = Path.Combine(directory, $"{baseFileName}.1{extension}");
            File.Move(_logFilePath, rotatedFileName);

            // Create a new log file
            using (FileStream fs = File.Create(_logFilePath)) { }

            // Optionally, limit the number of historical log files
            CleanUpOldLogFiles(logFileInfo);
        }
    }


    private void CleanUpOldLogFiles(FileInfo logFileInfo)
    {
        Setting setting = FileUtil.GetSetting();
        string? directory = logFileInfo.DirectoryName;
        string baseFileName = Path.GetFileNameWithoutExtension(logFileInfo.FullName);
        string extension = logFileInfo.Extension;

        if (directory != null)
        {
            DirectoryInfo di = new(directory);
            FileInfo[] logFiles = di.GetFiles($"{baseFileName}.*{extension}")
                                    .OrderByDescending(f => f.Name)
                                    .ToArray();

            if (logFiles.Length > setting.MaxLogFiles)
            {
                foreach (FileInfo file in logFiles.Skip(setting.MaxLogFiles))
                {
                    file.Delete();
                }
            }
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