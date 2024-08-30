using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Attributes;
using StreamMaster.Domain.Models;
using StreamMaster.Streams.Domain.Statistics;

namespace StreamMaster.Streams.Domain.Models
{
    [RequireAll]
    [TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]

    public class ChannelMetric
    {
        public SMStreamInfo? SMStreamInfo { get; set; }
        public long ChannelItemBackLog { get; set; }

        public List<ClientChannelDto> ClientChannels { get; set; } = [];

        public List<ClientStreamsDto> ClientStreams { get; set; }

        public StreamHandlerMetrics Metrics { get; set; }

        public bool IsFailed { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public string SourceName { get; set; }
        public string? Logo { get; set; }
        public string? VideoInfo { get; set; }
    }
}
