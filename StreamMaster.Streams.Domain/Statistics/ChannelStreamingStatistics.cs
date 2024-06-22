using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Extensions;

namespace StreamMaster.Streams.Domain.Statistics;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class ChannelStreamingStatistics : BPSStatistics
{
    public ChannelStreamingStatistics()
    {
        IsSet = true;
    }

    public int Id { get; set; }
    public string ChannelName { get; set; } = string.Empty;
    public string ChannelUrl { get; set; } = string.Empty;
    public int CurrentRank { get; set; }
    public string CurrentStreamId { get; set; } = string.Empty;
    public string? ChannelLogo { get; set; }
    public List<StreamStreamingStatistic> StreamStreamingStatistics { get; set; }

    public void SetStreamInfo(SMChannelDto smChannelDto, int currentRank, string currentStreamId)
    {
        StartTime = SMDT.UtcNow;

        CurrentRank = currentRank;
        ChannelUrl = smChannelDto.RealUrl;
        ChannelName = smChannelDto.Name;
        Id = smChannelDto.Id;
        ChannelLogo = smChannelDto.Logo;

        CurrentStreamId = currentStreamId;

    }
}
