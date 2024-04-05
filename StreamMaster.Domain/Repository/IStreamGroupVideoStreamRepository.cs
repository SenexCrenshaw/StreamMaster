using StreamMaster.Domain.API;
using StreamMaster.Domain.Pagination;

namespace StreamMaster.Domain.Repository;
public interface IStreamGroupVideoStreamRepository : IRepositoryBase<StreamGroupVideoStream>
{
    Task<StreamGroupDto?> SyncVideoStreamToStreamGroup(int StreamGroupId, string VideoStreamId);
    Task<PagedResponse<VideoStreamDto>> GetPagedStreamGroupVideoStreams(StreamGroupVideoStreamParameters Parameters);

    Task<List<VideoStreamIsReadOnly>> GetStreamGroupVideoStreamIds(int StreamGroupId);
    Task AddStreamGroupVideoStreams(int StreamGroupId, List<string> toAdd, bool IsReadOnly);
    Task RemoveStreamGroupVideoStreams(int StreamGroupId, IEnumerable<string> toRemove);
    Task SetStreamGroupVideoStreamsIsReadOnly(int StreamGroupId, List<string> toUpdate, bool IsReadOnly);
    Task SetVideoStreamRanks(int StreamGroupId, List<VideoStreamIDRank> videoStreamIDRanks);
    Task<List<VideoStreamDto>> GetStreamGroupVideoStreams(int StreamGroupId);
}