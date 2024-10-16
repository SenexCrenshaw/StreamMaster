using StreamMaster.Domain.Attributes;

namespace StreamMaster.Domain.Dto;

[RequireAll]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class M3UFileDto : BaseFileDto, IMapFrom<M3UFile>
{
    public M3UField M3UName { get; set; } = M3UField.Name;
    public M3UKey M3UKey { get; set; } = M3UKey.URL;
    public bool SyncChannels { get; set; }
    public List<string> VODTags { get; set; } = [];
    public string? DefaultStreamGroupName { get; set; }
    public int MaxStreamCount { get; set; }

    //public M3UFileStreamURLPrefix StreamURLPrefix { get; set; }
    public int StreamCount { get; set; }

    public int StartingChannelNumber { get; set; }
    public bool AutoSetChannelNumbers { get; set; }
}