using StreamMaster.Streams.Domain.Statistics;

using System.Xml.Serialization;

namespace StreamMaster.Streams.Domain.Interfaces;
public interface IStreamStats
{
    double GetAverageLatency();
    long GetBytesRead();
    double GetKbps();
    DateTime GetStartTime();
    /// <summary>
    /// Gets the metrics for the stream handler.
    /// </summary>
    [XmlIgnore]
    StreamHandlerMetrics Metrics { get; }
}