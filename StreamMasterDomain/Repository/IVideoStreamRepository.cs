using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterDomain.Repository
{
    public interface IVideoStreamRepository : IRepositoryBase<VideoStream>
    {
        IQueryable<VideoStream> GetVideoStreamsById(string id);
        IQueryable<string> GetVideoStreamNames();
        Task<IEnumerable<VideoStream>> DeleteVideoStreamsByM3UFiledId(int M3UFileId, CancellationToken cancellationToken);

        Task<bool> DeleteVideoStreamAsync(string videoStreamId, CancellationToken cancellationToken);

        IQueryable<VideoStream> GetVideoStreamsHidden();

        IQueryable<VideoStream> GetVideoStreamsByM3UFileId(int m3uFileId);

        Task SetGroupNameByGroupName(string channelGroupName, string newGroupName, CancellationToken cancellationToken);

        Task SetGroupVisibleByGroupName(string channelGroupName, bool isHidden, CancellationToken cancellationToken);

        Task<VideoStreamDto?> UpdateVideoStreamAsync(VideoStreamUpdate request, CancellationToken cancellationToken);

        Task<bool> SynchronizeChildRelationships(VideoStream videoStream, List<ChildVideoStreamDto> childVideoStreams, CancellationToken cancellationToken);

        IQueryable<VideoStream> GetVideoStreamsByMatchingIds(IEnumerable<string> ids);

        Task<PagedResponse<VideoStreamDto>> GetVideoStreamsForChannelGroups(VideoStreamParameters VideoStreamParameters, CancellationToken cancellationToken);

        IQueryable<VideoStream> GetAllVideoStreams();

        //Task<PagedResponse<VideoStreamDto>> GetVideoStreamsAsync(VideoStreamParameters VideoStreamParameters, CancellationToken cancellationToken);

        Task<VideoStream> GetVideoStreamByIdAsync(string Id, CancellationToken cancellationToken = default);

        Task<(VideoStreamHandlers videoStreamHandler, List<ChildVideoStreamDto> childVideoStreamDtos)?> GetStreamsFromVideoStreamById(string videoStreamId, CancellationToken cancellationToken = default);

        Task<VideoStreamDto> GetVideoStreamDtoByIdAsync(string Id, CancellationToken cancellationToken = default);

        void CreateVideoStream(VideoStream VideoStream);

        void UpdateVideoStream(VideoStream VideoStream);

        //void DeleteVideoStream(VideoStream VideoStream);
    }
}