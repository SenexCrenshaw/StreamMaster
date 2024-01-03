using StreamMaster.Domain.Attributes;
using StreamMaster.Domain.Models;

namespace StreamMaster.Domain.Dto;

[RequireAll]
public class M3UFileDto : BaseFileDto, IMapFrom<M3UFile>
{
    public List<string> VODTags { get; set; }
    public bool OverwriteChannelNumbers { get; set; }
    public int StartingChannelNumber { get; set; }
    public int MaxStreamCount { get; set; }
    //public M3UFileStreamURLPrefix StreamURLPrefix { get; set; }
    public int StationCount { get; set; }

}
