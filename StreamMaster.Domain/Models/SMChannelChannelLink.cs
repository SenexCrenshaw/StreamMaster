namespace StreamMaster.Domain.Models;

public class SMChannelChannelLink
{
    public int ParentSMChannelId { get; set; }
    public SMChannel ParentSMChannel { get; set; }

    public int SMChannelId { get; set; }
    public SMChannel SMChannel { get; set; }
    public int Rank { get; set; }
}