namespace StreamMaster.Domain.Models;

public class SMChannelStreamLink
{
    public int SMChannelId { get; set; }
    public SMChannel SMChannel { get; set; }

    public string SMStreamId { get; set; }
    public SMStream SMStream { get; set; }
    public int Rank { get; set; }
}