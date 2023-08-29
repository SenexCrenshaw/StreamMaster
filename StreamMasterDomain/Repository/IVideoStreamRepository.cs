using MediatR;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

using System.Threading;

namespace StreamMasterDomain.Repository
{
    public interface IVideoStreamRepository : IRepositoryBase<VideoStream>
    {
        Task<List<string>> GetVideoStreamVideoStreamIds(string videoStreamId, CancellationToken cancellationToken);
        Task<List<ChildVideoStreamDto>> GetVideoStreamVideoStreams(string videoStreamId, CancellationToken cancellationToken);
        IQueryable<VideoStream> GetJustVideoStreams();
        Task<bool> CreateVideoStreamAsync(CreateVideoStreamRequest request, CancellationToken cancellationToken);
        IQueryable<VideoStream> GetVideoStreamsById(string id);
        IQueryable<string> GetVideoStreamNames();
        Task<IEnumerable<VideoStream>> DeleteVideoStreamsByM3UFiledId(int M3UFileId, CancellationToken cancellationToken);

        Task<bool> DeleteVideoStreamAsync(string videoStreamId, CancellationToken cancellationToken);

        IQueryable<VideoStream> GetVideoStreamsHidden();

        IQueryable<VideoStream> GetVideoStreamsByM3UFileId(int m3uFileId);

        Task<int> SetGroupNameByGroupName(string channelGroupName, string newGroupName, CancellationToken cancellationToken);

        Task<int> SetGroupVisibleByGroupName(string channelGroupName, bool isHidden, CancellationToken cancellationToken);

        Task<bool> UpdateAllVideoStreamsFromParameters(VideoStreamParameters Parameters, UpdateVideoStreamRequest request, CancellationToken cancellationToken);
        Task<bool> UpdateVideoStreamAsync(UpdateVideoStreamRequest request, CancellationToken cancellationToken);

        Task<bool> SynchronizeChildRelationships(VideoStream videoStream, List<ChildVideoStreamDto> childVideoStreams, CancellationToken cancellationToken);

        IQueryable<VideoStream> GetVideoStreamsByMatchingIds(IEnumerable<string> ids);

        Task<PagedResponse<VideoStreamDto>> GetVideoStreams(VideoStreamParameters VideoStreamParameters, CancellationToken cancellationToken);

        IQueryable<VideoStream> GetAllVideoStreams();

        Task<VideoStream> GetVideoStreamByIdAsync(string Id, CancellationToken cancellationToken = default);

        Task<(VideoStreamHandlers videoStreamHandler, List<ChildVideoStreamDto> childVideoStreamDtos)?> GetStreamsFromVideoStreamById(string videoStreamId, CancellationToken cancellationToken = default);

        Task<VideoStreamDto> GetVideoStreamDtoByIdAsync(string Id, CancellationToken cancellationToken = default);

    }
}