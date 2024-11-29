namespace StreamMaster.Domain.Services;

public interface IM3UFileService
{
    Task<DataResponse<List<M3UFileDto>>> GetM3UFilesNeedUpdatingAsync();

    Task<M3UFile?> GetM3UFileAsync(int Id);

    Task<M3UFile?> ProcessM3UFile(int M3UFileId, bool ForceRun = false);

    Task UpdateM3UFile(M3UFile m3uFile);

    Task<List<M3UFile>> GetM3UFilesAsync();

    (M3UFile m3uFile, string fullName) CreateM3UFile(string Name, int? MaxStreamCount, string? M3U8OutPutProfile, M3UKey? M3UKey, M3UField? M3UName, string? DefaultStreamGroupName, string? UrlSource, bool? SyncChannels, int? HoursToUpdate, int? StartingChannelNumber, bool? AutoSetChannelNumbers, List<string>? VODTags);

    (M3UFile m3uFile, string fullName) CreateM3UFile(string Name, int? MaxStreamCount, string? M3U8OutPutProfile, M3UKey? M3UKey, M3UField? M3UName, int? StartingChannelNumber, bool? AutoSetChannelNumbers, string? DefaultStreamGroupName, int? HoursToUpdate, bool? SyncChannels, IFormFile? FormFile, List<string>? VODTags);

    (string fullName, string fullNameWithExtension) GetFileName(string name);
}