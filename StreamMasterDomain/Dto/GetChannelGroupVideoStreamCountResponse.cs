namespace StreamMasterDomain.Dto;

public class GetChannelGroupVideoStreamCountResponse
{
    public int Id { get; set; }
    public int ActiveCount { get; set; }
    public int TotalCount { get; set; }
    public int HiddenCount { get; set; }
}