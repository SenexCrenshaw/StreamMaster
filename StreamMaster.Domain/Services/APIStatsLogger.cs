using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

using StreamMaster.Domain.Configuration;

namespace StreamMaster.Domain.Services;

public class APIStatsLogger : IAPIStatsLogger
{
    private readonly ILogger<APIStatsLogger> logger;
    private readonly IOptionsMonitor<Setting> settings;
    private readonly string logFilePath;
    private readonly SemaphoreSlim fileLock = new(1, 1);

    public APIStatsLogger(ILogger<APIStatsLogger> logger, IOptionsMonitor<Setting> settings)
    {
        this.logger = logger;
        this.settings = settings;

        // Ensure the directory exists
        Directory.CreateDirectory(BuildInfo.APIStatsFolder);

        // Create a new log file with a timestamp for this app instance
        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        logFilePath = Path.Combine(BuildInfo.APIStatsFolder, $"stats_{timestamp}.csv");

        // Write CSV headers if the file doesn't exist
        if (!File.Exists(logFilePath))
        {
            File.AppendAllText(logFilePath, "Timestamp,Function,Size(Bytes),ElapsedTime(MS),Message\n", Encoding.UTF8);
        }

        // Rotate logs to keep only the last 10 files
        RotateLogs();
    }

    public async Task<T> DebugAPI<T>(Task<T> task, [CallerMemberName] string callerName = "")
    {
        if (settings.CurrentValue.DebugAPI)
        {
            logger.LogInformation("{callerName} starting", callerName);
            Stopwatch stopwatch = Stopwatch.StartNew();

            T? result = await task.ConfigureAwait(false);

            stopwatch.Stop();

            try
            {
                int byteSize = JsonSerializer.SerializeToUtf8Bytes(result).Length;
                long elapsedTime = stopwatch.ElapsedMilliseconds;

                logger.LogInformation("{callerName} retrieved size: {byteSize} bytes, in {elapsedTime} ms", callerName, byteSize, elapsedTime);

                // Write structured data to the CSV log
                await WriteToLogFileAsync(callerName, byteSize.ToString(), elapsedTime.ToString(), "Success");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to serialize debug data in {callerName}: {ex.Message}", callerName, ex.Message);

                // Log error details to the CSV file
                await WriteToLogFileAsync(callerName, "0", "0", $"Error: {ex.Message}");
            }

            return result;
        }

        return await task.ConfigureAwait(false);
    }

    public async Task WriteToLogFileAsync(string functionName, string size, string elapsedTime, string message)
    {
        string logEntry = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff},{functionName},{size},{elapsedTime},{message}";

        // Use a semaphore to ensure thread-safe file access
        await fileLock.WaitAsync();
        try
        {
            await File.AppendAllTextAsync(logFilePath, logEntry + Environment.NewLine, Encoding.UTF8);
        }
        finally
        {
            fileLock.Release();
        }
    }

    private static void RotateLogs()
    {
        List<FileInfo> logFiles = [.. new DirectoryInfo(BuildInfo.APIStatsFolder)
            .GetFiles("stats_*.csv")
            .OrderByDescending(f => f.CreationTime)
            .Skip(10)];

        foreach (FileInfo? file in logFiles)
        {
            file.Delete();
        }
    }
}
