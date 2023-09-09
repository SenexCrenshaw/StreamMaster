using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterDomain.Repository;
public interface IStreamGroupVideoStreamRepository : IRepositoryBase<StreamGroupVideoStream, StreamGroupVideoStream>
{
    Task<StreamGroupDto?> SyncVideoStreamToStreamGroup(int StreamGroupId, string VideoStreamId, CancellationToken cancellationToken = default);
    Task<PagedResponse<VideoStreamDto>> GetStreamGroupVideoStreams(StreamGroupVideoStreamParameters Parameters, CancellationToken cancellationToken = default);

    Task<List<VideoStreamIsReadOnly>> GetStreamGroupVideoStreamIds(int id, CancellationToken cancellationToken = default);
    Task AddStreamGroupVideoStreams(int StreamGroupId, List<string> toAdd, bool IsReadOnly, CancellationToken cancellationToken);
    Task RemoveStreamGroupVideoStreams(int StreamGroupId, IEnumerable<string> toRemove, CancellationToken cancellationToken);
    Task SetStreamGroupVideoStreamsIsReadOnly(int StreamGroupId, List<string> toUpdate, bool IsReadOnly, CancellationToken cancellationToken);
    Task SetVideoStreamRanks(int StreamGroupId, List<VideoStreamIDRank> videoStreamIDRanks, CancellationToken cancellationToken);
    Task<List<VideoStream>> GetStreamGroupVideoStreamsList(int StreamGroupId, CancellationToken cancellationToken);
}