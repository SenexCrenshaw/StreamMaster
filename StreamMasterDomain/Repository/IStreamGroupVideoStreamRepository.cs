using StreamMasterDomain.Dto;
using StreamMasterDomain.Models;
using StreamMasterDomain.Pagination;

namespace StreamMasterDomain.Repository;
public interface IStreamGroupVideoStreamRepository : IRepositoryBase<StreamGroupVideoStream>
{
    Task<StreamGroupDto?> SyncVideoStreamToStreamGroup(int StreamGroupId, string VideoStreamId, CancellationToken cancellationToken);
    Task<PagedResponse<VideoStreamDto>> GetPagedStreamGroupVideoStreams(StreamGroupVideoStreamParameters Parameters, CancellationToken cancellationToken);

    Task<List<VideoStreamIsReadOnly>> GetStreamGroupVideoStreamIds(int StreamGroupId, CancellationToken cancellationToken);
    Task AddStreamGroupVideoStreams(int StreamGroupId, List<string> toAdd, bool IsReadOnly, CancellationToken cancellationToken);
    Task RemoveStreamGroupVideoStreams(int StreamGroupId, IEnumerable<string> toRemove, CancellationToken cancellationToken);
    Task SetStreamGroupVideoStreamsIsReadOnly(int StreamGroupId, List<string> toUpdate, bool IsReadOnly, CancellationToken cancellationToken);
    Task SetVideoStreamRanks(int StreamGroupId, List<VideoStreamIDRank> videoStreamIDRanks, CancellationToken cancellationToken);
    Task<List<VideoStreamDto>> GetStreamGroupVideoStreams(int StreamGroupId, CancellationToken cancellationToken);
}