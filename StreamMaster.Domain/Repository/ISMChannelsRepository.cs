using StreamMaster.Domain.API;
using StreamMaster.Domain.Pagination;

using System.Linq.Expressions;

namespace StreamMaster.Domain.Repository;

public interface ISMChannelsRepository : IRepositoryBase<SMChannel>
{
    new IQueryable<SMChannel> GetQuery(Expression<Func<SMChannel, bool>> expression, bool tracking = false);
    Task CreateSMChannel(SMChannel sMChannel);
    PagedResponse<SMChannelDto> CreateEmptyPagedResponse();
    Task<PagedResponse<SMChannelDto>> GetPagedSMChannels(QueryStringParameters parameters);
    new IQueryable<SMChannel> GetQuery(bool tracking = false);
    List<SMChannelDto> GetSMChannels();
    Task<APIResponse> DeleteSMChannel(int smchannelId);
    Task<List<int>> DeleteSMChannelsFromParameters(QueryStringParameters parameters);
    SMChannel? GetSMChannel(int smchannelId);
    Task<APIResponse> CreateSMChannelFromStream(string streamId);
    Task<APIResponse> DeleteSMChannels(List<int> smchannelIds);
    Task<APIResponse> AddSMStreamToSMChannel(int SMChannelId, string SMStreamId);
    Task<APIResponse> RemoveSMStreamFromSMChannel(int SMChannelId, string SMStreamId);
    Task<APIResponse> SetSMStreamRanks(List<SMChannelRankRequest> request);
    Task<APIResponse> SetSMChannelLogo(int SMChannelId, string logo);
    Task<APIResponse> SetSMChannelChannelNumber(int sMChannelId, int channelNumber);
    Task<APIResponse> SetSMChannelName(int sMChannelId, string name);
    Task<APIResponse> SetSMChannelEPGID(int sMChannelId, string EPGId);
    Task<APIResponse> SetSMChannelGroup(int sMChannelId, string group);
    Task<APIResponse> CopySMChannel(int sMChannelId, string newName);
    Task<APIResponse> CreateSMChannelFromStreams(List<string> streamIds);
    Task<APIResponse> CreateSMChannelFromStreamParameters(QueryStringParameters parameters);
    Task<List<FieldData>> ToggleSMChannelsVisibleById(List<int> ids, CancellationToken cancellationToken);
    Task<SMChannelDto?> ToggleSMChannelVisibleById(int id, CancellationToken cancellationToken);
    Task<List<FieldData>> ToggleSMChannelVisibleByParameters(QueryStringParameters parameters, CancellationToken cancellationToken);
    Task<List<FieldData>> AutoSetEPGFromParameters(QueryStringParameters parameters, CancellationToken cancellationToken);
    Task<List<FieldData>> AutoSetEPGFromIds(List<int> ids, CancellationToken cancellationToken);
    Task<List<FieldData>> SetSMChannelsLogoFromEPGFromIds(List<int> ids, CancellationToken cancellationToken);
    Task<List<FieldData>> SetSMChannelsLogoFromEPGFromParameters(QueryStringParameters parameters, CancellationToken cancellationToken);
    Task<APIResponse> SetSMChannelProxy(int sMChannelId, int streamingProxy);
}