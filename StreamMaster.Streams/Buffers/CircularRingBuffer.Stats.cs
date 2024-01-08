using Prometheus;

namespace StreamMaster.Infrastructure.VideoStreamManager.Buffers;

public sealed partial class CircularRingBuffer : ICircularRingBuffer
{

    private static readonly Gauge _waitTime = Metrics.CreateGauge(
  "sm_circular_buffer_read_wait_for_data_availability_duration_milliseconds",
        "Client waiting duration in milliseconds for data availability",
new GaugeConfiguration
{
    LabelNames = ["circular_buffer_id", "client_id", "video_stream_name"]

});

    private static readonly Gauge _bitsPerSecond = Metrics.CreateGauge(
"sm_circular_buffer_read_stream_bits_per_second",
"Bits per second read from the input stream.",
new GaugeConfiguration
{
    LabelNames = ["circular_buffer_id", "video_stream_name"]
});

    private static readonly Counter _bytesWrittenCounter = Metrics.CreateCounter(
        "sm_circular_buffer_bytes_written_total",
        "Total number of bytes written.",
        new CounterConfiguration
        {
            LabelNames = ["circular_buffer_id", "video_stream_name"]
        });

    private static readonly Counter _writeErrorsCounter = Metrics.CreateCounter(
        "sm_circular_buffer_write_errors_total",
        "Total number of write errors.",
         new CounterConfiguration
         {
             LabelNames = ["circular_buffer_id", "video_stream_name"]
         });

    private static readonly Gauge _dataArrival = Metrics.CreateGauge(
"sm_circular_buffer_arrival_time_milliseconds",
    "Data arrival times in milliseconds.",
new GaugeConfiguration
{
    LabelNames = ["circular_buffer_id", "video_stream_name"]
});


}
