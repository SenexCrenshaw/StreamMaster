using Reinforced.Typings.Attributes;

namespace StreamMaster.Application.Settings.Queries;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class SDSystemStatus
{
    public bool IsSystemReady { get; init; }
}
