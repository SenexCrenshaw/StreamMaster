using StreamMaster.Domain.Attributes;
using StreamMaster.Domain.Helpers;

namespace StreamMaster.Domain.Dto;

[RequireAll]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class SMStreamInfo
{
    public static SMStreamInfo NewSMStreamInfo(string name, bool isCustomStream)
    {
        return new SMStreamInfo
        {
            Id = name,
            Name = name,
            Url = "",
            M3UFileId = EPGHelper.CustomPlayListId,
            IsCustomStream = isCustomStream
        };
    }
    public required string Id { get; set; }
    public string? ClientUserAgent { get; set; } = null;
    public required string Name { get; set; } = string.Empty;
    public required string Url { get; set; } = string.Empty;
    public int? SecondsIn { get; set; } = null;
    public required int M3UFileId { get; set; }
    public bool IsCustomStream { get; set; } = false;
}