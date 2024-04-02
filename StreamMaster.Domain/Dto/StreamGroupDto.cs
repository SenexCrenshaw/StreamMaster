using Reinforced.Typings.Attributes;

namespace StreamMaster.Domain.Dto;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class StreamGroupDto : IMapFrom<StreamGroup>
{
    public string FFMPEGProfileId { get; set; } = string.Empty;
    public bool IsLoading { get; set; } = false;
    public string HDHRLink { get; set; } = string.Empty;
    public bool IsReadOnly { get; set; } = false;
    public bool AutoSetChannelNumbers { get; set; } = false;
    public int StreamCount { get; set; } = 0;

    public int Id { get; set; }
    public string M3ULink { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string XMLLink { get; set; } = string.Empty;
    public string ShortM3ULink { get; set; } = string.Empty;
    public string ShortEPGLink { get; set; } = string.Empty;
}
