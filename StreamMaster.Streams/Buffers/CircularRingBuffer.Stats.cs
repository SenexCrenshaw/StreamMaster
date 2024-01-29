using Prometheus;

using StreamMaster.Domain.Cache;

namespace StreamMaster.Infrastructure.VideoStreamManager.Buffers;

public sealed partial class CircularRingBuffer : ICircularRingBuffer
{

    private static readonly Gauge _bitsPerSecond = Metrics.CreateGauge(
        "sm_circular_buffer_read_stream_bits_per_second",
        "Bits per second read from the input stream.",
        new GaugeConfiguration
        {
            LabelNames = ["video_stream_name"]
        }
    );

    private static readonly Counter _bytesWrittenCounter = Metrics.CreateCounter(
        "sm_circular_buffer_bytes_written_total",
        "Total number of bytes written.",
        new CounterConfiguration
        {
            LabelNames = ["video_stream_name"]
        }
    );

    private static readonly Counter _writeErrorsCounter = Metrics.CreateCounter(
        "sm_circular_buffer_write_errors_total",
        "Total number of write errors.",
        new CounterConfiguration
        {
            LabelNames = ["video_stream_name"]
        }
    );

    private static readonly Gauge _dataArrival = Metrics.CreateGauge(
        "sm_circular_buffer_arrival_time_milliseconds",
        "Data arrival times in milliseconds.",
        new GaugeConfiguration
        {
            LabelNames = ["video_stream_name"]
        }
    );

    private DateTime _lastUpdateTime = DateTime.UtcNow;
    private int acculmativeBytesWritten = 0;
    private void SetMetrics(int bytesWritten)
    {
        DateTime currentTime = DateTime.UtcNow;
        _writeMetric.RecordBytesProcessed(bytesWritten);

        Setting setting = memoryCache.GetSetting();

        if (setting.EnablePrometheus && (currentTime - _lastUpdateTime > TimeSpan.FromSeconds(5)))
        {
            _bytesWrittenCounter.WithLabels(StreamInfo.VideoStreamName).Inc(acculmativeBytesWritten);
            _bitsPerSecond.WithLabels(StreamInfo.VideoStreamName).Set(_writeMetric.GetBitsPerSecond());
            _inputStreamStatistics.AddBytesWritten(acculmativeBytesWritten);
            _lastUpdateTime = currentTime;
            acculmativeBytesWritten = 0;
        }

        acculmativeBytesWritten += bytesWritten;
    }
}
