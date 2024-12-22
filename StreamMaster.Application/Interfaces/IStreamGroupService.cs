using Microsoft.AspNetCore.Http;

using StreamMaster.Application.StreamGroups;

namespace StreamMaster.Application.Interfaces;

public interface IStreamGroupService
{
    List<int>? DecodeProfileIds(string encryptedString);
    string EncodeProfileIds(List<int> streamGroupProfileIds);
    Task<Dictionary<int, SGFS>> GetSMFS(List<int> sgProfileIds, bool isShort, CancellationToken cancellationToken);
    Task SyncSTRMFilesAsync(CancellationToken cancellationToken);
    Task SyncSGSTRMFilesAsync(StreamGroup streamGroup, CancellationToken cancellationToken);

    Task<bool> StreamGroupExistsAsync(int streamGroupProfileId);

    string GetStreamGroupLineupStatus();

    Task<int> GetDefaultSGIdAsync();

    Task<StreamGroup> GetDefaultSGAsync();

    Task<(int? StreamGroupId, int? StreamGroupProfileId, int? SMChannelId)> DecodeProfileIdSMChannelIdFromEncodedAsync(string encodedString);

    Task<CommandProfileDto> GetProfileFromSGIdsCommandProfileNameAsync(int? streamGroupId, int streamGroupProfileId, string commandProfileName);

    Task<int> GetStreamGroupIdFromSGProfileIdAsync(int? streamGroupProfileId);
    Task<(int? StreamGroupId, int? StreamGroupProfileId)> DecodeStreamGroupIdProfileIdFromEncodedAsync(string encodedString);
    string EncodeStreamGroupIdProfileIdChannelId(StreamGroup streamGroup, int streamGroupProfileId, int smChannelId);
    Task<string?> EncodeStreamGroupIdProfileIdAsync(int streamGroupId, int streamGroupProfileId);
    Task<string?> EncodeStreamGroupIdProfileIdChannelIdAsync(int streamGroupId, int streamGroupProfileId, int smChannelId);

    Task<StreamGroupProfile> GetDefaultStreamGroupProfileAsync();

    Task<string> GetStreamGroupCapabilityAsync(int streamGroupProfileId, HttpRequest request);

    Task<string> GetStreamGroupDiscoverAsync(int streamGroupProfileId, HttpRequest request);

    Task<StreamGroup?> GetStreamGroupFromIdAsync(int streamGroupId);

    Task<StreamGroup?> GetStreamGroupFromNameAsync(string streamGroupName);

    Task<StreamGroup?> GetStreamGroupFromSGProfileIdAsync(int streamGroupProfileId);

    Task<string> GetStreamGroupLineupAsync(int streamGroupProfileId, bool IsShort);

    Task<StreamGroupProfile> GetStreamGroupProfileAsync(int? streamGroupId = null, int? streamGroupProfileId = null);

    Task<(List<VideoStreamConfig> videoStreamConfigs, StreamGroupProfile streamGroupProfile)> GetStreamGroupVideoConfigsAsync(int streamGroupProfileId);
}