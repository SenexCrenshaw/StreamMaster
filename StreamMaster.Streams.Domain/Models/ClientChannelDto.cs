using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Attributes;
using StreamMaster.Streams.Domain.Statistics;

namespace StreamMaster.Streams.Domain.Models
{
    [RequireAll]
    [TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]

    public class ClientChannelDto
    {
        public int SMChannelId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Logo { get; set; }
        //public string? ClientId { get; set; }
        public string? ClientIPAddress { get; set; }
        public string? ClientUserAgent { get; set; }
        public StreamHandlerMetrics? Metrics { get; set; }
    }
}
