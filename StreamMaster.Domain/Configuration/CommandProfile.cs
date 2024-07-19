namespace StreamMaster.Domain.Configuration;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class CommandProfile
{
    public bool IsReadOnly { get; set; } = false;
    public string Command { get; set; } = "ffmpeg";
    public string Parameters { get; set; } = "";
    //public int Timeout { get; set; } = 20;
    //public bool IsM3U8 { get; set; } = false;

}

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class CommandProfileDto : CommandProfile
{
    public string ProfileName { get; set; } = "";
}


public class CommandProfileList
{
    public Dictionary<string, CommandProfile> CommandProfiles { get; set; } = [];
}