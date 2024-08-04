using StreamMaster.Domain.Attributes;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Helpers;

namespace StreamMaster.Domain.Dto;

[RequireAll]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class SMStreamInfo
{
    public static SMStreamInfo NewSMStreamInfo(string id, string name, CommandProfileDto CommandProfile, bool isCustomStream)
    {
        return new SMStreamInfo
        {
            Id = id,
            Name = name,
            Url = "",
            M3UFileId = EPGHelper.CustomPlayListId,
            IsCustomStream = isCustomStream,
            CommandProfile = CommandProfile
        };
    }
    public required CommandProfileDto CommandProfile { get; set; }
    public required string Id { get; set; }
    public string? ClientUserAgent { get; set; } = null;
    public required string Name { get; set; } = string.Empty;
    public required string Url { get; set; } = string.Empty;
    public int? SecondsIn { get; set; } = null;
    public required int M3UFileId { get; set; }
    public bool IsCustomStream { get; set; } = false;
}