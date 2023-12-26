using StreamMaster.Domain.Models;

namespace StreamMaster.Domain.Dto;

public class ChannelLogoDto : IconFile
{
    public string EPGId { get; set; }
    public int EPGFileId { get; set; }

}
