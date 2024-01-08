using Prometheus;

namespace StreamMaster.Streams.Buffers;

public sealed partial class ClientReadStream
{

    private readonly Gauge _bitsPerSecond = Metrics.CreateGauge("sm_client_read_stream_bits_per_second", "Bits per second read from the client stream.",
        new GaugeConfiguration
        {
            LabelNames = ["client_id", "circular_buffer_id", "video_stream_name"]
        }
    );


    private readonly Gauge _readDuration = Metrics.CreateGauge("sm_client_read_stream_duration_milliseconds", "Duration of read operations in milliseconds.",
        new GaugeConfiguration
        {
            LabelNames = ["client_id", "circular_buffer_id", "video_stream_name"]
        }
    );

    private readonly Counter _bytesReadCounter = Metrics.CreateCounter("sm_client_read_stream_bytes_read_total", "Total number of bytes read.",
        new CounterConfiguration
        {
            LabelNames = ["client_id", "circular_buffer_id", "video_stream_name"]
        }
    );

    private readonly Counter _readErrorsCounter = Metrics.CreateCounter("sm_client_read_stream_errors_total", "Total number of read errors.",
        new CounterConfiguration
        {
            LabelNames = ["client_id", "circular_buffer_id", "video_stream_name"]
        }
    );

    private readonly Counter _readCancellationCounter = Metrics.CreateCounter("sm_client_read_stream_cancellations_total", "Total number of read cancellations.",
        new CounterConfiguration
        {
            LabelNames = ["client_id", "circular_buffer_id", "video_stream_name"]
        }
    );

    private readonly PerformanceBpsMetrics metrics = new();

}