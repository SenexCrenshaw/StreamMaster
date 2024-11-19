namespace StreamMaster.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true)]
public class StreamGroupProfile
{
    public int Id { get; set; }
    public int StreamGroupId { get; set; }
    public string ProfileName { get; set; } = "Default";
    public string OutputProfileName { get; set; } = "Default";
    public string CommandProfileName { get; set; } = "Default";
}


[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true)]
public class StreamGroupProfileDto : StreamGroupProfileLinks, IMapFrom<StreamGroupProfile>
{
    public required string ShortHDHRLink { get; set; }
    public required string ShortM3ULink { get; set; }
    public required string ShortEPGLink { get; set; }
}


[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true)]
public class StreamGroupProfileLinks : StreamGroupProfile
{
    public string XMLLink { get; set; } = string.Empty;
    public string M3ULink { get; set; } = string.Empty;
    public string HDHRLink { get; set; } = string.Empty;

}
