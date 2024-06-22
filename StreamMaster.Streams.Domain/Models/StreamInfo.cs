using MessagePack;

using Reinforced.Typings.Attributes;

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
                foreach (var stream in SMChannel.SMStreams.OrderBy(a => a.Rank))
                {

                    ret.Add(new StreamStreamingStatistic
                    {
                        Id = stream.Id,
                        StreamName = stream.Name,
                        StreamLogo = stream.Logo,
                        Rank = stream.Rank
                    });
                }
            }

            return ret;
        }
    }
}
