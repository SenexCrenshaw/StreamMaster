using Reinforced.Typings.Attributes;

namespace StreamMaster.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true)]
public class StreamGroupSMChannel
{
    public SMChannel SMChannel { get; set; }
    public int SMChannelId { get; set; }

    public bool IsReadOnly
    {
        get; set;
    }

    public int StreamGroupId { get; set; }
    public int Rank { get; set; }
}

