using StreamMaster.Domain.API;
using StreamMaster.Domain.Pagination;

using System.Linq.Expressions;

namespace StreamMaster.Domain.Repository;

public interface ISMChannelsRepository : IRepositoryBase<SMChannel>
{
    Task<List<FieldData>> AutoSetEPGs(List<SMChannel> smChannels, bool skipSave, CancellationToken cancellationToken);
    Task<IdIntResultWithResponse> AutoSetSMChannelNumbersFromParameters(int StreamGroupId, QueryStringParameters Parameters, int? StartingNumber, bool? OverwriteExisting);
    Task<IdIntResultWithResponse> AutoSetSMChannelNumbersRequest(int StreamGroupId, List<int> SMChannelIds, int? StartingNumber, bool? OverwriteExisting);
    //IQueryable<SMChannel> GetPagedSMChannelsQueryable(QueryStringParameters Parameters);
    IQueryable<SMChannel> GetPagedSMChannelsQueryable(QueryStringParameters parameters, bool? tracking = false);
    Task<APIResponse> AddSMStreamToSMChannel(int SMChannelId, string SMStreamId, int? Rank);
    Task<List<FieldData>> AutoSetEPGFromIds(List<int> ids, CancellationToken cancellationToken);
    Task<List<FieldData>> AutoSetEPGFromParameters(QueryStringParameters Parameters, CancellationToken cancellationToken);
    Task ChangeGroupName(string oldGroupName, string newGroupName);
    Task<APIResponse> CloneSMChannel(int sMChannelId, string newName);
    PagedResponse<SMChannelDto> CreateEmptyPagedResponse();

    Task CreateSMChannel(SMChannel sMChannel);

    Task<APIResponse> CreateSMChannelsFromStreamParameters(QueryStringParameters Parameters, int? AddToStreamGroupId);

    Task<APIResponse> CreateSMChannelsFromStreams(List<string> streamIds, int? AddToStreamGroupId);

    Task<APIResponse> DeleteSMChannel(int smchannelId);

    Task<APIResponse> DeleteSMChannels(List<int> smchannelIds);

    Task<List<int>> DeleteSMChannelsFromParameters(QueryStringParameters Parameters);

    Task<PagedResponse<SMChannelDto>> GetPagedSMChannels(QueryStringParameters Parameters);

    new IQueryable<SMChannel> GetQuery(Expression<Func<SMChannel, bool>> expression, bool tracking = false);

    new IQueryable<SMChannel> GetQuery(bool tracking = false);

    SMChannel? GetSMChannel(int smchannelId);
    SMChannel? GetSMChannelFromStreamGroup(int smChannelId, int streamGroupNumber);
    List<SMChannelDto> GetSMChannels();

    Task<List<SMChannel>> GetSMChannelsFromStreamGroup(int streamGroupId);

    Task<APIResponse> RemoveSMStreamFromSMChannel(int SMChannelId, string SMStreamId);

    Task<APIResponse> SetSMChannelChannelNumber(int sMChannelId, int channelNumber);

    Task<APIResponse> SetSMChannelEPGID(int sMChannelId, string EPGId);

    Task<APIResponse> SetSMChannelGroup(int sMChannelId, string group);

    Task<APIResponse> SetSMChannelLogo(int SMChannelId, string logo);

    Task<APIResponse> SetSMChannelName(int sMChannelId, string name);

    Task<APIResponse> SetSMChannelCommandProfileName(int sMChannelId, string CommandProfileName);

    Task<List<FieldData>> SetSMChannelsLogoFromEPGFromIds(List<int> ids, CancellationToken cancellationToken);

    Task<List<FieldData>> SetSMChannelsLogoFromEPGFromParameters(QueryStringParameters Parameters, CancellationToken cancellationToken);

    Task<APIResponse> SetSMStreamRanks(List<SMChannelRankRequest> request);

    Task<List<FieldData>> ToggleSMChannelsVisibleById(List<int> ids, CancellationToken cancellationToken);

    Task<SMChannelDto?> ToggleSMChannelVisibleById(int id, CancellationToken cancellationToken);

    Task<List<FieldData>> ToggleSMChannelVisibleByParameters(QueryStringParameters Parameters, CancellationToken cancellationToken);
    Task<APIResponse> SetSMChannelsGroup(List<int> sMChannelIds, string GroupName);
    Task<APIResponse> SetSMChannelsGroupFromParameters(QueryStringParameters Parameters, string GroupName);
    Task<APIResponse> SetSMChannelsCommandProfileName(List<int> sMChannelIds, string CommandProfileName);
    Task<APIResponse> SetSMChannelsCommandProfileNameFromParameters(QueryStringParameters parameters, string CommandProfileName);
    //Task<APIResponse> CreateSMChannelsFromCustomStreams(List<string> smStreamIds);
}