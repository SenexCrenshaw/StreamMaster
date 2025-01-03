using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Attributes;
using StreamMaster.Domain.Models;
using StreamMaster.Streams.Domain.Metrics;

namespace StreamMaster.Streams.Domain.Models
{
    [RequireAll]
    [TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]

    public class ChannelMetric
    {
        public SMStreamInfo? SMStreamInfo { get; set; }
        //public long ChannelItemBackLog { get; set; }

        //public List<ClientChannelDto> ClientChannels { get; set; } = [];

        public List<ClientStreamsDto> ClientStreams { get; set; } = [];

        public StreamHandlerMetrics Metrics { get; set; } = new();

        public bool IsFailed { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string SourceName { get; set; } = string.Empty;
        public string? ChannelLogo { get; set; }
        public string? StreamLogo { get; set; }
        public string? VideoInfo { get; set; }
        //public long TotalBytesInBuffer { get; set; }
    }
}
