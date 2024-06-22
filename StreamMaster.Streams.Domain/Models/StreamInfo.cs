using MessagePack;

using Reinforced.Typings.Attributes;

namespace StreamMaster.Streams.Domain.Models;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class StreamInfo
{

    [IgnoreMember]
    public int Rank { get; set; }

    [IgnoreMember]
    public SMStreamDto SMStream { get; set; }

    [IgnoreMember]
    public SMChannelDto SMChannel { get; set; }
}
