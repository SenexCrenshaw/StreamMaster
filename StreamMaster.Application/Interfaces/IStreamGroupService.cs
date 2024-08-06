using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.Interfaces;

public interface IStreamGroupService
{
    #region CRYPTO
    Task<(int? StreamGroupId, int? StreamGroupProfileId, int? SMChannelId)> DecodeProfileIdSMChannelIdFromEncodedAsync(string EncodedString);

    Task<(int? StreamGroupId, int? StreamGroupProfileId, int? SMChannelId)> DecodeStreamGroupIdProfileIdChannelId(string encodedString);
    Task<(int? StreamGroupId, int? StreamGroupProfileId, string? SMStreamId)> DecodeStreamGroupIdProfileIdStreamId(string encodedString);
    string EncodeStreamGroupIdProfileIdChannelId(StreamGroup streamGroup, int StreamGroupProfileId, int SMChannelId);
    Task<string?> EncodeStreamGroupIdStreamIdAsync(int StreamGroupId, string SMStreamId);
    Task<string?> EncodeStreamGroupIdProfileIdChannelId(int StreamGroupId, int StreamGroupProfileId, int SMChannelId);
    Task<string?> EncodeStreamGroupIdProfileIdStreamId(int StreamGroupId, int StreamGroupProfileId, string SMStreamId);

    Task<string?> EncodeStreamGroupProfileIdChannelId(int StreamGroupProfileId, int SMChannelId);
    #endregion

    string GetStreamGroupLineupStatus();
    Task<string> GetStreamGroupLineup(int StreamGroupProfileId, HttpRequest httpRequest, bool IsShort);
    Task<string> GetStreamGroupDiscover(int StreamGroupProfileId, HttpRequest reques);
    Task<string> GetStreamGroupCapability(int StreamGroupProfileId, HttpRequest request);
    Task<(List<VideoStreamConfig> videoStreamConfigs, StreamGroupProfile streamGroupProfile)> GetStreamGroupVideoConfigs(int StreamGroupProfileId);
    Task<StreamGroup?> GetStreamGroupFromIdAsync(int streamGroupId);
    Task<StreamGroup?> GetStreamGroupFromNameAsync(string streamGroupName);
    Task<StreamGroup> GetDefaultSGAsync();
    Task<int> GetDefaultSGIdAsync();
    Task<StreamGroupProfile> GetDefaultStreamGroupProfileAsync();
    Task<CommandProfileDto> GetProfileFromSGIdsCommandProfileNameAsync(int? streamGroupId, int streamGroupProfileId, string CommandProfileName);
    Task<StreamGroupProfile> GetStreamGroupProfileAsync(int? StreamGroupId = null, int? StreamGroupProfileId = null);
    Task<int> GetStreamGroupIdFromSGProfileIdAsync(int? streamGroupProfileId);
    Task<StreamGroup?> GetStreamGroupFromSGProfileIdAsync(int streamGroupProfileId);
    Task<List<StreamGroupDto>> GetStreamGroups();
}