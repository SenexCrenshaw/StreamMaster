using Reinforced.Typings.Attributes;

namespace StreamMaster.Domain.Dto;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class SMChannelDto : SMChannel, IMapFrom<SMChannel>
{
    public new List<SMStreamDto> SMStreams { get; set; } = [];
    public string RealUrl { get; set; } = string.Empty;
}
