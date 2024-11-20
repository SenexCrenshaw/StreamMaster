namespace StreamMaster.Domain.Dto;

public class ChannelLogoDto : IconFile
{
    public required string EPGId { get; set; }
    public int EPGFileId { get; set; }
}
