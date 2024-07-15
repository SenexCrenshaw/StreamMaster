[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class VideoOutputProfile
{
    public bool IsReadOnly { get; set; } = false;
    public string Command { get; set; } = "ffmpeg";
    public string Parameters { get; set; } = "";
    public int Timeout { get; set; } = 20;
    public bool IsM3U8 { get; set; } = false;

}

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class VideoOutputProfileDto : VideoOutputProfile
{
    public string ProfileName { get; set; } = "";
}


public class VideoOutputProfiles
{
    public Dictionary<string, VideoOutputProfile> VideoProfiles { get; set; } = [];
}