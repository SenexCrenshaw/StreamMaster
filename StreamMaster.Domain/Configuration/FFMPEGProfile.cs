using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Attributes;

namespace StreamMaster.Domain.Configuration;

[RequireAll]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class FFMPEGProfile
{
    public string Parameters { get; set; } = "";
    public int Timeout { get; set; } = 20;
    public bool IsM3U8 { get; set; } = true;
}

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]

public class FFMPEGProfileDto : FFMPEGProfile
{
    public string Name { get; set; } = "";
}

//[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
//public class List<FFMPEGProfileDto> : List<FFMPEGProfileDto>
//{
//}


