using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Attributes;

namespace StreamMaster.Domain.Dto;


[RequireAll]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class M3UFileDto : BaseFileDto, IMapFrom<M3UFile>
{
    public List<string> VODTags { get; set; }
    public bool OverwriteChannelNumbers { get; set; }
    public int StartingChannelNumber { get; set; }
    public int MaxStreamCount { get; set; }
    //public M3UFileStreamURLPrefix StreamURLPrefix { get; set; }
    public int StationCount { get; set; }

}
