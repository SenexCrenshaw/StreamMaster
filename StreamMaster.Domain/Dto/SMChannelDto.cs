namespace StreamMaster.Domain.Dto;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
//[MessagePackObject(keyAsPropertyName: true)]
public class SMChannelDto : SMChannel, IMapFrom<SMChannel>
{
    public new List<SMStreamDto> SMStreams { get; set; } = [];
    public List<int> StreamGroupIds { get; set; } = [];
    public string StreamUrl { get; set; } = string.Empty;
    public int CurrentRank { get; set; } = -1;
}
