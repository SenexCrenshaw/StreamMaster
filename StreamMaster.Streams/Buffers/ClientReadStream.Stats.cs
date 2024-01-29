using Prometheus;

using StreamMaster.Domain.Cache;

namespace StreamMaster.Streams.Buffers;

public sealed partial class ClientReadStream
{

    private static readonly Gauge _bitsPerSecond = Metrics.CreateGauge("sm_client_read_stream_bits_per_second", "Bits per second read from the client stream.",
        new GaugeConfiguration
        {
            LabelNames = ["client_id", "video_stream_name"]
        }
    );


    private static readonly Counter _bytesReadCounter = Metrics.CreateCounter("sm_client_read_stream_bytes_read_total", "Total number of bytes read.",
        new CounterConfiguration
        {
            LabelNames = ["client_id", "video_stream_name"]
        }
    );

    private static readonly Counter _readErrorsCounter = Metrics.CreateCounter("sm_client_read_stream_errors_total", "Total number of read errors.",
        new CounterConfiguration
        {
            LabelNames = ["client_id", "video_stream_name"]
        }
    );

    private readonly PerformanceBpsMetrics metrics = new();
    private DateTime _lastUpdateTime = DateTime.UtcNow;
    private int acculmativeBytesRead = 0;
    private void SetMetrics(int bytesRead)
    {
        DateTime currentTime = DateTime.UtcNow;
        Setting setting = memoryCache.GetSetting();

        if (setting.EnablePrometheus && (currentTime - _lastUpdateTime > TimeSpan.FromSeconds(5)))
        {
            double bps = metrics.GetBitsPerSecond();

            _bitsPerSecond.WithLabels(ClientId.ToString(), Buffer.VideoStreamName).Set(bps);
            _bytesReadCounter.WithLabels(ClientId.ToString(), Buffer.VideoStreamName).Inc(acculmativeBytesRead);

            acculmativeBytesRead = 0;
            _lastUpdateTime = currentTime;
        }

        acculmativeBytesRead += bytesRead;
    }

}