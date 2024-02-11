using StreamMaster.Domain.Cache;
using StreamMaster.Domain.Extensions;

namespace StreamMaster.Streams.Streams;


/// <summary>
/// Manages the streaming of a single video stream, including client registrations and circularRingbuffer handling.
/// </summary>
public sealed partial class StreamHandler
{
    private DateTime _lastUpdateTime = SMDT.UtcNow;
    private int acculmativeBytesWritten = 0;
    private void SetMetrics(int bytesWritten)
    {
        DateTime currentTime = SMDT.UtcNow;

        Setting setting = memoryCache.GetSetting();

        if (setting.EnablePrometheus && (currentTime - _lastUpdateTime > TimeSpan.FromSeconds(5)))
        {
            inputStreamStatistics.AddBytesWritten(acculmativeBytesWritten);
            _lastUpdateTime = currentTime;
            acculmativeBytesWritten = 0;
        }

        acculmativeBytesWritten += bytesWritten;
    }
}