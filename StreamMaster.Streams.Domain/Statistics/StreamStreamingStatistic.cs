using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Extensions;

namespace StreamMaster.Streams.Domain.Statistics;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class StreamStreamingStatistic : BPSStatistics
{
    public int Rank { get; set; }
    public string StreamName { get; set; } = string.Empty;
    public string? StreamLogo { get; set; }
    public string? StreamUrl { get; set; }
    public string Id { get; set; } = string.Empty;

    public void SetStream(SMStreamDto smStream)
    {
        Rank = smStream.Rank;
        StreamName = smStream.Name;
        StreamLogo = smStream.Logo;
        StartTime = SMDT.UtcNow;
        StreamUrl = smStream.Url;
        Id = smStream.Id;
    }

    public new StreamStreamingStatistic Copy()
    {
        return new StreamStreamingStatistic
        {
            Rank = Rank,
            StreamName = StreamName,
            StreamLogo = StreamLogo,
            Id = Id,

            BytesRead = BytesRead,
            BytesWritten = BytesWritten,
            BitsPerSecond = BitsPerSecond,
            StreamUrl = StreamUrl,
            StartTime = StartTime
        };
    }
}
