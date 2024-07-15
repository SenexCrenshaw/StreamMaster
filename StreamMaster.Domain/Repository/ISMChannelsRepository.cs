using StreamMaster.Domain.API;
using StreamMaster.Domain.Pagination;

using System.Linq.Expressions;

namespace StreamMaster.Domain.Repository;

public interface ISMChannelsRepository : IRepositoryBase<SMChannel>
{
    Task<IdIntResultWithResponse> AutoSetSMChannelNumbersFromParameters(int StreamGroupId, QueryStringParameters Parameters, int? StartingNumber, bool? OverwriteExisting);
    Task<IdIntResultWithResponse> AutoSetSMChannelNumbersRequest(int StreamGroupId, List<int> SMChannelIds, int? StartingNumber, bool? OverwriteExisting);
    IQueryable<SMChannel> GetPagedSMChannelsQueryable(QueryStringParameters parameters);
    Task<APIResponse> AddSMStreamToSMChannel(int SMChannelId, string SMStreamId, int? Rank);
    Task<List<FieldData>> AutoSetEPGFromIds(List<int> ids, CancellationToken cancellationToken);
    Task<List<FieldData>> AutoSetEPGFromParameters(QueryStringParameters parameters, CancellationToken cancellationToken);
    Task ChangeGroupName(string oldGroupName, string newGroupName);
    Task<APIResponse> CopySMChannel(int sMChannelId, string newName);
    PagedResponse<SMChannelDto> CreateEmptyPagedResponse();

    Task CreateSMChannel(SMChannel sMChannel);

    Task<APIResponse> CreateSMChannelFromStream(string streamId, int? StreamGroupId, int? M3UFileId);

    Task<APIResponse> CreateSMChannelFromStreamParameters(QueryStringParameters parameters, int? StreamGroupId, int? M3UFileId);

    Task<APIResponse> CreateSMChannelFromStreams(List<string> streamIds, int? StreamGroupId, int? M3UFileId);

    Task<APIResponse> DeleteSMChannel(int smchannelId);

    Task<APIResponse> DeleteSMChannels(List<int> smchannelIds);

    Task<List<int>> DeleteSMChannelsFromParameters(QueryStringParameters parameters);

    Task<PagedResponse<SMChannelDto>> GetPagedSMChannels(QueryStringParameters parameters);

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

    Task<APIResponse> SetSMChannelProxy(int sMChannelId, string streamingProxy);

    Task<List<FieldData>> SetSMChannelsLogoFromEPGFromIds(List<int> ids, CancellationToken cancellationToken);

    Task<List<FieldData>> SetSMChannelsLogoFromEPGFromParameters(QueryStringParameters parameters, CancellationToken cancellationToken);

    Task<APIResponse> SetSMStreamRanks(List<SMChannelRankRequest> request);

    Task<List<FieldData>> ToggleSMChannelsVisibleById(List<int> ids, CancellationToken cancellationToken);

    Task<SMChannelDto?> ToggleSMChannelVisibleById(int id, CancellationToken cancellationToken);

    Task<List<FieldData>> ToggleSMChannelVisibleByParameters(QueryStringParameters parameters, CancellationToken cancellationToken);
    Task<APIResponse> SetSMChannelsGroup(List<int> sMChannelIds, string GroupName);
    Task<APIResponse> SetSMChannelsGroupFromParameters(QueryStringParameters parameters, string GroupName);
}