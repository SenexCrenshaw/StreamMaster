namespace StreamMaster.WebDav.Services;

/// <summary>
/// Tracks performance metrics and operational logs.
/// </summary>
public class MonitoringService : IMonitoringService
{
    public void TrackApiCall(string endpoint, TimeSpan duration, bool success)
    {
        Console.WriteLine($"API Call: {endpoint}, Duration: {duration.TotalMilliseconds} ms, Success: {success}");
    }

    public void TrackCacheHit(string path)
    {
        Console.WriteLine($"Cache Hit: {path}");
    }

    public void TrackCacheMiss(string path)
    {
        Console.WriteLine($"Cache Miss: {path}");
    }

    public void TrackFileRead(string path, bool isVirtual)
    {
        Console.WriteLine($"File Read: {path}, Virtual: {isVirtual}");
    }

    public void TrackFileWrite(string path)
    {
        Console.WriteLine($"File Written: {path}");
    }
}