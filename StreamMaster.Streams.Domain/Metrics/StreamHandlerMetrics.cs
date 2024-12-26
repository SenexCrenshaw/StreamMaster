using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Attributes;

namespace StreamMaster.Streams.Domain.Metrics;

[RequireAll]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class StreamHandlerMetrics : IStreamHandlerMetrics
{
    public long BytesRead { get; set; }
    public long BytesWritten { get; set; }
    public double Kbps { get; set; }
    public DateTime StartTime { get; set; }
    public double AverageLatency { get; set; }
    public int ErrorCount { get; set; }
}
