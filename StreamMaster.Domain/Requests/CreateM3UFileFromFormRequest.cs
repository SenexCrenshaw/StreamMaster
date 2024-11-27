
namespace StreamMaster.Domain.Requests;

[SMAPI(JustController = true, JustHub = true)]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateM3UFileFromFormRequest(string Name, int? MaxStreamCount, string? M3U8OutPutProfile, M3UKey? M3UKey, M3UField? M3UName, int? StartingChannelNumber, bool? AutoSetChannelNumbers, string? DefaultStreamGroupName, int? HoursToUpdate, bool? SyncChannels, IFormFile? FormFile, List<string>? VODTags)
    : IRequest<APIResponse>;