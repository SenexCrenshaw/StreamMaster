namespace StreamMaster.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class VideoInfo
{
    public DateTime Created { get; set; }
    public string Key { get; set; } = string.Empty;
    public string StreamName { get; set; } = string.Empty;
    public string StreamId { get; set; } = string.Empty;
    public string JsonOutput { get; set; } = string.Empty;
}

public class VideoInfoDto : VideoInfo
{
    public VideoInfoDto() { }
    public VideoInfoDto(KeyValuePair<string, VideoInfo> info)
    {
        Key = info.Key;
        StreamName = info.Value.StreamName;
        JsonOutput = info.Value.JsonOutput;
        StreamId = info.Value.StreamId;
    }
}