[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class FileOutputProfile
{

    public bool IsReadOnly { get; set; } = false;
    public EPGOutputProfile EPGOutputProfile { get; set; } = new();
    public M3UOutputProfile M3UOutputProfile { get; set; } = new();

}
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class EPGOutputProfile
{

}

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class EPGOutputProfileRequest
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

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class M3UOutputProfileRequest
{

    public bool? EnableIcon { get; set; }
    public string? TVGName { get; set; }
    public string? ChannelId { get; set; }
    public string? TVGId { get; set; }
    public string? TVGGroup { get; set; }
    public string? ChannelNumber { get; set; }
    public string? GroupTitle { get; set; }
}


public class FileOutputProfiles
{
    public Dictionary<string, FileOutputProfile> FileProfiles { get; set; } = [];
}

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class FileOutputProfileDto : FileOutputProfile
{
    public string Name { get; set; } = "";
}
