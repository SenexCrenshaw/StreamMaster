using StreamMaster.Domain.API;
using StreamMaster.Domain.Pagination;

namespace StreamMaster.Domain.Repository;

public interface ISMChannelsRepository
{
    Task CreateSMChannel(SMChannel sMChannel);
    PagedResponse<SMChannelDto>? CreateEmptyPagedResponse();
    Task<PagedResponse<SMChannelDto>> GetPagedSMChannels(SMChannelParameters parameters);
    IQueryable<SMChannel> GetQuery(bool tracking = false);
    List<SMChannelDto> GetSMChannels();
    Task DeleteSMChannel(int smchannelId);
    Task<List<int>> DeleteAllSMChannelsFromParameters(SMChannelParameters parameters);
    SMChannel? GetSMChannel(int smchannelId);
    Task<DefaultAPIResponse> CreateSMChannelFromStream(string streamId);
    Task<DefaultAPIResponse> DeleteSMChannels(List<int> smchannelIds);
    Task<DefaultAPIResponse> AddSMStreamToSMChannel(int SMChannelId, string SMStreamId);
    Task<DefaultAPIResponse> RemoveSMStreamFromSMChannel(int SMChannelId, string SMStreamId);
}