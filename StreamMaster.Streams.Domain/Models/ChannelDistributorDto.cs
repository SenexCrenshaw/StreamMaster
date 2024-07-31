using StreamMaster.Streams.Domain.Statistics;

namespace StreamMaster.Streams.Domain.Models
{
    public class ClientChannelDto
    {
        public int SMChannelId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Logo { get; set; }
    }

    public class ClientStreamsDto
    {
        public string SMStreamId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Logo { get; set; }
    }


    public class ChannelDistributorDto
    {
        public SMStreamInfo SMStreamInfo { get; set; }

        public long GetChannelItemCount { get; set; }

        public List<ClientChannelDto> ClientChannels { get; set; } = [];

        public List<ClientStreamsDto> ClientStreams { get; set; }

        public StreamHandlerMetrics GetMetrics { get; set; }

        public bool IsFailed { get; set; }
        public string Name { get; set; }
    }
}