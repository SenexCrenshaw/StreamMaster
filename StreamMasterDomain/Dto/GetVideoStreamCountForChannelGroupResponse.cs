namespace StreamMasterDomain.Dto;

public class GetVideoStreamCountForChannelGroupResponse
{
    public int Id { get; set; }
    public int ActiveCount { get; set; }
    public int TotalCount { get; set; }
    public int HiddenCount { get; set; }
}