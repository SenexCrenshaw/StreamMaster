using StreamMasterDomain.Common;
using StreamMasterDomain.Services;

using StreamMasterInfrastructure.Services;

namespace StreamMasterInfrastructure.Logger;

public class FileLoggingServiceFactory : IFileLoggingServiceFactory
{
    public IFileLoggingService Create(string key)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string debugLogPath = Path.Combine(BuildInfo.LogFolder, $"StreamMasterAPI_{timestamp}_debug.log");

        string logFilePath = key switch
        {
            "FileLogger" => BuildInfo.LogFilePath,
            "FileLoggerDebug" => Path.Combine(debugLogPath),
            _ => throw new ArgumentException("Invalid key for file logging service", nameof(key)),
        };

        return new FileLoggingService(logFilePath);
    }
}
