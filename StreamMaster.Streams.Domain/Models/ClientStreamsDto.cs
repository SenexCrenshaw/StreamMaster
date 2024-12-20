using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Attributes;
using StreamMaster.Streams.Domain.Statistics;

namespace StreamMaster.Streams.Domain.Models
{
    [RequireAll]
    [TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]

    public class ClientStreamsDto
    {
        public StreamHandlerMetrics? Metrics { get; set; }
        public string SMStreamId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Logo { get; set; }
        public string? ClientIPAddress { get; set; }
        public string? ClientUserAgent { get; set; }
        public string SMChannelId { get; set; } = string.Empty;
    }
}
