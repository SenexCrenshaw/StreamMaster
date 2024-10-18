using Reinforced.Typings.Attributes;

namespace StreamMaster.Domain.Dto;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class SDSystemStatus
{
    public bool IsSystemReady { get; init; }
}
