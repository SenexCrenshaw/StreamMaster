[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class OutputProfile
{
    public static string APIName => "GetOutputProfiles";

    public bool IsReadOnly { get; set; } = false;
    public bool EnableIcon { get; set; } = true;
    public bool EnableId { get; set; } = true;
    public bool EnableGroupTitle { get; set; } = true;

    public bool EnableChannelNumber { get; set; } = true;

    public string Name { get; set; } = string.Empty;
    public string EPGId { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    //public string ChannelNumber { get; set; } = string.Empty;


}

//[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record OutputProfileRequest
{
    public bool? EnableIcon { get; set; }
    public bool? EnableGroupTitle { get; set; }
    public bool? EnableId { get; set; }
    public bool? EnableChannelNumber { get; set; }

    public string? Name { get; set; }
    public string? EPGId { get; set; }
    public string? Group { get; set; }
    //public string? ChannelNumber { get; set; }

}


public class OutputProfiles
{
    public Dictionary<string, OutputProfile> OutProfiles { get; set; } = [];
}

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class OutputProfileDto : OutputProfile
{
    public string ProfileName { get; set; } = "";
}
