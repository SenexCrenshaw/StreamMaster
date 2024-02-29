namespace StreamMaster.Domain.Models;

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

