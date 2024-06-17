using System.Text.Json;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class FileOutputProfile
{
    public static string APIName => "FileOutputProfiles";

    private readonly JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };
    public bool IsReadOnly { get; set; } = false;
    public EPGOutputProfile EPGOutputProfile { get; set; } = new();
    public M3UOutputProfile M3UOutputProfile { get; set; } = new();

}
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class EPGOutputProfile
{

}

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class M3UOutputProfile
{

    public bool EnableIcon { get; set; } = true;
    public string TVGName { get; set; } = string.Empty;
    public string ChannelId { get; set; } = string.Empty;
    public string TVGId { get; set; } = string.Empty;
    public string TVGGroup { get; set; } = string.Empty;
    public string ChannelNumber { get; set; } = string.Empty;
    public string GroupTitle { get; set; } = string.Empty;
}


public class FileOutputProfiles
{
    public Dictionary<string, FileOutputProfile> FileProfiles { get; set; } = [];
}


public class FileOutputProfileDto : FileOutputProfile
{
    public string Name { get; set; } = "";
}
