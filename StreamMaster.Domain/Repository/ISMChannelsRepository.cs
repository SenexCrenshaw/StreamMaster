using System.Linq.Expressions;

using StreamMaster.Domain.Pagination;

namespace StreamMaster.Domain.Repository;

public interface ISMChannelsRepository : IRepositoryBase<SMChannel>
{
    // Channel Creation and Deletion
    Task CreateSMChannel(SMChannel sMChannel);
    Task<APIResponse> CreateSMChannelsFromStreamParameters(QueryStringParameters Parameters, int? AddToStreamGroupId);
    Task<APIResponse> CreateSMChannelsFromStreams(List<string> streamIds, int? AddToStreamGroupId);
    Task<APIResponse> DeleteSMChannel(int smChannelId);
    Task<APIResponse> DeleteSMChannels(List<int> smChannelIds);
    Task<List<int>> DeleteSMChannelsFromParameters(QueryStringParameters Parameters);

    // Channel Retrieval
    SMChannel? GetSMChannel(int smChannelId);
    Task<SMChannel?> GetSMChannelFromStreamGroupAsync(int smChannelId, int streamGroupId);
    List<SMChannelDto> GetSMChannels();
    Task<List<SMChannel>> GetSMChannelsFromStreamGroup(int streamGroupId);
    PagedResponse<SMChannelDto> CreateEmptyPagedResponse();
    Task<PagedResponse<SMChannelDto>> GetPagedSMChannels(QueryStringParameters Parameters);
    Task<IQueryable<SMChannel>> GetPagedSMChannelsQueryableAsync(QueryStringParameters parameters, bool? tracking = false);

    // Custom Queries
    new IQueryable<SMChannel> GetQuery(Expression<Func<SMChannel, bool>> expression, bool tracking = false);
    new IQueryable<SMChannel> GetQuery(bool tracking = false);

    // Channel Number Management
    Task<IdIntResultWithResponse> AutoSetSMChannelNumbersFromParameters(int streamGroupId, QueryStringParameters parameters, int? startingNumber, bool? overwriteExisting);
    Task<IdIntResultWithResponse> AutoSetSMChannelNumbersRequest(int streamGroupId, List<int> SMChannelIds, int? startingNumber, bool? overwriteExisting);
    Task<APIResponse> SetSMChannelChannelNumber(int sMChannelId, int channelNumber);

    // Channel EPG Management
    Task<List<FieldData>> AutoSetEPGs(IQueryable<SMChannel> smChannels, bool skipSave, CancellationToken cancellationToken);
    Task<List<FieldData>> AutoSetEPGFromIds(List<int> ids, CancellationToken cancellationToken);
    Task<List<FieldData>> AutoSetEPGFromParameters(QueryStringParameters parameters, CancellationToken cancellationToken);
    Task<APIResponse> SetSMChannelEPGID(int sMChannelId, string EPGId);

    // Channel Group Management
    Task ChangeGroupName(string oldGroupName, string newGroupName);
    Task<APIResponse> SetSMChannelGroup(int sMChannelId, string group);
    Task<APIResponse> SetSMChannelsGroup(List<int> SMChannelIds, string groupName);
    Task<APIResponse> SetSMChannelsGroupFromParameters(QueryStringParameters parameters, string groupName);

    // Channel Command Profile Management
    Task<APIResponse> SetSMChannelCommandProfileName(int sMChannelId, string commandProfileName);
    Task<APIResponse> SetSMChannelsCommandProfileName(List<int> sMChannelIds, string commandProfileName);
    Task<APIResponse> SetSMChannelsCommandProfileNameFromParameters(QueryStringParameters parameters, string commandProfileName);

    // Channel Visibility Toggles
    Task<List<FieldData>> ToggleSMChannelsVisibleById(List<int> ids, CancellationToken cancellationToken);
    Task<SMChannelDto?> ToggleSMChannelVisibleById(int id, CancellationToken cancellationToken);
    Task<List<FieldData>> ToggleSMChannelVisibleByParameters(QueryStringParameters parameters, CancellationToken cancellationToken);

    // Channel Cloning
    Task<APIResponse> CloneSMChannel(int SMChannelId, string newName);

    // Channel SMLogoUrl Management
    Task<APIResponse> SetSMChannelLogo(int SMChannelId, string logo);
    Task<List<FieldData>> SetSMChannelsLogoFromEPGFromIds(List<int> ids, CancellationToken cancellationToken);
    Task<List<FieldData>> SetSMChannelsLogoFromEPGFromParameters(QueryStringParameters parameters, CancellationToken cancellationToken);

    // Channel Name Management
    Task<APIResponse> SetSMChannelName(int sMChannelId, string name);
}
