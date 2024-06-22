using MessagePack;

using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Extensions;
using StreamMaster.Streams.Domain.Statistics;

namespace StreamMaster.Streams.Domain.Models;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class StreamInfo
{

    [IgnoreMember]
    public int CurrentRank { get; set; }

    [IgnoreMember]
    public SMStreamDto SMStream { get; set; }

    [IgnoreMember]
    public SMChannelDto SMChannel { get; set; }
    public List<StreamStreamingStatistic> StreamStatistics
    {
        get
        {
            var ret = new List<StreamStreamingStatistic>();
            if (SMChannel != null && SMChannel.SMStreams != null)
            {
                foreach (var smStream in SMChannel.SMStreams.OrderBy(a => a.Rank))
                {
                    ret.Add(new StreamStreamingStatistic
                    {
                        StreamName = smStream.Name,
                        StreamLogo = smStream.Logo,
                        StartTime = SMDT.UtcNow,
                        StreamUrl = smStream.Url,
                        Id = smStream.Id
                    });
                }
            }

            return ret;
        }
    }
}
