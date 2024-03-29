using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Attributes;

namespace StreamMaster.Domain.Dto;

[RequireAll]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]

public class ChannelGroupDto : ChannelGroupArg, IMapFrom<ChannelGroup>
{
    public int Id { get; set; }
}