using Reinforced.Typings.Attributes;

namespace StreamMaster.Domain.Dto;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
//[MessagePackObject(keyAsPropertyName: true)]
public class SMChannelDto : SMChannel, IMapFrom<SMChannel>
{
    public new List<SMStreamDto> SMStreams { get; set; } = [];
    public new List<int> StreamGroupIds { get; set; } = [];
    public string RealUrl { get; set; } = string.Empty;
    public int Rank { get; set; }
}
