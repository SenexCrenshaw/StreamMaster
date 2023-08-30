using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

using System.Threading.Tasks;

namespace StreamMasterDomain.Repository
{
    public interface IStreamGroupVideoStreamRepository : IRepositoryBase<StreamGroup>
    {
        Task AddVideoStreamToStreamGroup(int StreamGroupId, string VideoStreamId, CancellationToken cancellationToken = default);

        Task RemoveVideoStreamFromStreamGroup(int StreamGroupId, string VideoStreamId, CancellationToken cancellationToken = default);

        Task<PagedResponse<VideoStreamDto>> GetStreamGroupVideoStreams(StreamGroupVideoStreamParameters Parameters, CancellationToken cancellationToken = default);

        Task<List<VideoStreamIsReadOnly>> GetStreamGroupVideoStreamIds(int id, CancellationToken cancellationToken = default);
    }
}