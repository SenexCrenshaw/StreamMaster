using StreamMaster.Domain.Mappings;
using StreamMaster.Domain.Models;

using StreamMaster.Domain.Attributes;

namespace StreamMaster.Domain.Dto;

[RequireAll]
public class M3UFileDto : BaseFileDto, IMapFrom<M3UFile>
{
    public int StartingChannelNumber { get; set; }
    public int MaxStreamCount { get; set; }
    //public M3UFileStreamURLPrefix StreamURLPrefix { get; set; }
    public int StationCount { get; set; }

}
