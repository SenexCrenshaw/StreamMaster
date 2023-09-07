using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterDomain.Repository;
public interface IStreamGroupVideoStreamRepository : IRepositoryBase<StreamGroupVideoStream>
{
    Task AddVideoStreamToStreamGroup(int StreamGroupId, string VideoStreamId, CancellationToken cancellationToken = default);

    Task RemoveVideoStreamFromStreamGroup(int StreamGroupId, string VideoStreamId, CancellationToken cancellationToken = default);

    Task<PagedResponse<VideoStreamDto>> GetStreamGroupVideoStreams(StreamGroupVideoStreamParameters Parameters, CancellationToken cancellationToken = default);

    Task<List<VideoStreamIsReadOnly>> GetStreamGroupVideoStreamIds(int id, CancellationToken cancellationToken = default);
    Task AddStreamGroupVideoStreams(int StreamGroupId, List<string> toAdd, bool IsReadOnly, CancellationToken cancellationToken);
    Task RemoveStreamGroupVideoStreams(int StreamGroupId, List<string> toRemove, CancellationToken cancellationToken);
    Task SetVideoStreamRanks(int StreamGroupId, List<VideoStreamIDRank> videoStreamIDRanks, CancellationToken cancellationToken);
}