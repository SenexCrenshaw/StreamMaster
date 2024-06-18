[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class OutputProfile
{
    public static string APIName => "OutputProfiles";

    public bool IsReadOnly { get; set; } = false;
    public bool EnableIcon { get; set; } = true;
    public string TVGName { get; set; } = string.Empty;
    public string ChannelId { get; set; } = string.Empty;
    public string TVGId { get; set; } = string.Empty;
    public string TVGGroup { get; set; } = string.Empty;
    public string ChannelNumber { get; set; } = string.Empty;
    public string GroupTitle { get; set; } = string.Empty;

}

//[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record OutputProfileRequest
{
    public bool? EnableIcon { get; set; }
    public string? TVGName { get; set; }
    public string? ChannelId { get; set; }
    public string? TVGId { get; set; }
    public string? TVGGroup { get; set; }
    public string? ChannelNumber { get; set; }
    public string? GroupTitle { get; set; }
}


public class OutputProfiles
{
    public Dictionary<string, OutputProfile> OutProfiles { get; set; } = [];
}

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class OutputProfileDto : OutputProfile
{
    public string Name { get; set; } = "";
}
