using Reinforced.Typings.Attributes;

namespace StreamMaster.Streams.Domain.Statistics;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class ClientStreamingStatistics : BPSStatistics
{

    public void SetStreamerConfiguration(IClientConfiguration StreamerConfiguration)
    {
        ClientIPAddress = StreamerConfiguration.ClientIPAddress;
        ChannelName = StreamerConfiguration.SMChannel.Name;
        ChannelId = StreamerConfiguration.SMChannel.Id;
        ClientAgent = StreamerConfiguration.ClientUserAgent;
        UniqueRequestId = StreamerConfiguration.UniqueRequestId;

    }


    public string ChannelName { get; set; } = string.Empty;
    public int ChannelId { get; set; }

    public string UniqueRequestId { get; set; } = string.Empty;
    public string ClientAgent { get; set; } = string.Empty;
    public string ClientIPAddress { get; set; } = string.Empty;

}
