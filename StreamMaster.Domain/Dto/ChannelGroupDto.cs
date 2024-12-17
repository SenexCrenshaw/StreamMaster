using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Attributes;

namespace StreamMaster.Domain.Dto;

[RequireAll]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]

public class ChannelGroupDto : ChannelGroup, IMapFrom<ChannelGroup>;