using StreamMaster.Domain.Attributes;

namespace StreamMaster.Domain.Requests;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateM3UFileRequest(string Name, int MaxStreamCount, string? M3U8OutPutProfile, M3UKey? M3UKey, M3UField? M3UName, string? DefaultStreamGroupName, string? UrlSource, bool? SyncChannels, int? HoursToUpdate, int? StartingChannelNumber, bool? AutoSetChannelNumbers, List<string>? VODTags) : IRequest<APIResponse>;