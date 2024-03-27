namespace StreamMaster.Domain.Dto;

public class SMChannelDto : SMChannel, IMapFrom<SMChannel>
{
    public new List<SMStreamDto> SMStreams { get; set; } = [];
    public string RealUrl { get; set; } = string.Empty;
}
