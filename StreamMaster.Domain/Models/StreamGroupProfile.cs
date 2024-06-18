namespace StreamMaster.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true)]
public class StreamGroupProfile
{
    public int Id { get; set; }
    public int StreamGroupId { get; set; }
    public string Name { get; set; }
    public string OutputProfileName { get; set; }
    public string VideoProfileName { get; set; }
}


[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true)]
public class StreamGroupProfileDto : StreamGroupProfileLinks, IMapFrom<StreamGroupProfile>
{

}


public class StreamGroupProfileLinks : StreamGroupProfile
{
    public string XMLLink { get; set; } = string.Empty;
    public string ShortM3ULink { get; set; } = string.Empty;
    public string ShortEPGLink { get; set; } = string.Empty;
    public string M3ULink { get; set; } = string.Empty;
    public string HDHRLink { get; set; } = string.Empty;

}
