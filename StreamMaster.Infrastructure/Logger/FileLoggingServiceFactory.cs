using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Extensions;
using StreamMaster.Infrastructure.Services;

namespace StreamMaster.Infrastructure.Logger;

public class FileLoggingServiceFactory(IOptionsMonitor<Setting> intSettings) : IFileLoggingServiceFactory
{
    public IFileLoggingService Create(string key)
    {
        string timestamp = SMDT.UtcNow.ToString("yyyyMMdd_HHmmss");
        string debugLogPath = Path.Combine(BuildInfo.LogFolder, $"StreamMasterAPI_{timestamp}_debug.log");

        string logFilePath = key switch
        {
            "SMLogger" => BuildInfo.LogFilePath,
            "FileLogger" => BuildInfo.LogFileLoggerPath,
            "FileLoggerDebug" => debugLogPath,
            _ => throw new ArgumentException("Invalid key for file logging service", nameof(key)),
        };

        return new FileLoggingService(logFilePath, intSettings);
    }
}