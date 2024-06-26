using Reinforced.Typings.Attributes;

namespace StreamMaster.Domain.Common;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class UpdateSettingResponse
{
    public bool NeedsLogOut { get; set; }
    public SettingDto Settings { get; set; }
}
