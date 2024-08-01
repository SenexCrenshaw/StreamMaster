namespace StreamMaster.Domain.Models;

public class SMChannelChannelLink
{
    public int ParentSMChannelId { get; set; }
    public SMChannel ParentSMChannel { get; set; }

    public int ChildSMChannelId { get; set; }
    public SMChannel ChildSMChannel { get; set; }
    public int Rank { get; set; }
}