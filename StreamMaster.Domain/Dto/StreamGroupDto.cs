namespace StreamMaster.Domain.Dto;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class StreamGroupDto : IMapFrom<StreamGroup>
{
    public List<StreamGroupProfileDto> StreamGroupProfiles { get; set; } = [];
    public bool IsLoading { get; set; } = false;
    public bool IsReadOnly { get; set; } = false;
    public int ChannelCount { get; set; } = 0;
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;


    public bool AutoSetChannelNumbers { get; set; } = true;
    public bool IgnoreExistingChannelNumbers { get; set; } = true;
    public int StartingChannelNumber { get; set; } = 1;

    public string ShortM3ULink { get; set; } = string.Empty;
    public string ShortEPGLink { get; set; } = string.Empty;
    public string M3ULink { get; set; } = string.Empty;
    public string XMLLink { get; set; } = string.Empty;
    public string HDHRLink { get; set; } = string.Empty;



}