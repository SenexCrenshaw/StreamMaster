namespace StreamMaster.Domain.Dto;

public class ChannelGroupStreamCount
{
    public int ChannelGroupId { get; set; }
    public int ActiveCount { get; set; }
    public int TotalCount { get; set; }
    public int HiddenCount { get; set; }
}