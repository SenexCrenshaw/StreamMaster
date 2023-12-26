namespace StreamMaster.Domain.Services;

public interface IFileLoggingService
{
    void EnqueueLogEntry(string format, params object[] args);
    void EnqueueLogEntry(string logEntry);
    Task StopLoggingAsync();
}