using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterDomain.Repository
{
    public interface IVideoStreamRepository : IRepositoryBase<VideoStream, VideoStreamDto>
    {
        Task<List<VideoStreamDto>> GetVideoStreamsForChannelGroups(List<int> channelGroupIds, CancellationToken cancellationToken);
        IQueryable<VideoStream> GetJustVideoStreams();

        Task<VideoStream?> CreateVideoStreamAsync(CreateVideoStreamRequest request, CancellationToken cancellationToken);

        IQueryable<VideoStream> GetVideoStreamsById(string VideoStreamId);

        IQueryable<string> GetVideoStreamNames();

        Task<IEnumerable<VideoStream>> DeleteVideoStreamsByM3UFiledId(int M3UFileId, CancellationToken cancellationToken);

        Task<VideoStreamDto?> DeleteVideoStreamAsync(string VideoStreamId, CancellationToken cancellationToken);

        IQueryable<VideoStream> GetVideoStreamsNotHidden();

        IQueryable<VideoStream> GetVideoStreamsByM3UFileId(int m3uFileId);

        Task<List<VideoStreamDto>> SetGroupNameByGroupName(string channelGroupName, string newGroupName, CancellationToken cancellationToken);
        Task<List<VideoStreamDto>> SetVideoStreamsLogoFromEPGFromIds(List<string> VideoStreamIds, CancellationToken cancellationToken);
        Task<List<VideoStreamDto>> ReSetVideoStreamsLogoFromIds(List<string> VideoStreamIds, CancellationToken cancellationToken);
        Task<List<VideoStreamDto>> ReSetVideoStreamsLogoFromParameters(VideoStreamParameters Parameters, CancellationToken cancellationToken);
        Task<List<VideoStreamDto>> SetGroupVisibleByGroupName(string channelGroupName, bool isHidden, CancellationToken cancellationToken);

        Task<List<VideoStreamDto>> SetVideoStreamChannelNumbersFromIds(List<string> VideoStreamIds, bool OverWriteExisting, int StartNumber, string OrderBy, CancellationToken cancellationToken);
        Task<List<VideoStreamDto>> SetVideoStreamChannelNumbersFromParameters(VideoStreamParameters Parameters, bool OverWriteExisting, int StartNumber, CancellationToken cancellationToken);

        Task<List<VideoStreamDto>> SetVideoStreamsLogoFromEPGFromParameters(VideoStreamParameters Parameters, CancellationToken cancellationToken);

        Task<List<VideoStreamDto>> SetVideoStreamSetEPGsFromName(List<string> VideoStreamIds, CancellationToken cancellationToken);

        Task<IEnumerable<string>> DeleteAllVideoStreamsFromParameters(VideoStreamParameters Parameters, CancellationToken cancellationToken);
        Task<(List<VideoStreamDto> videoStreams, bool updateChannelGroup)> UpdateAllVideoStreamsFromParameters(VideoStreamParameters Parameters, UpdateVideoStreamRequest request, CancellationToken cancellationToken);

        Task<(List<VideoStreamDto> videoStreams, bool updateChannelGroup)> UpdateVideoStreamsAsync(IEnumerable<UpdateVideoStreamRequest> VideoStreamUpdates, CancellationToken cancellationToken);
        Task<(VideoStreamDto? videoStream, bool updateChannelGroup)> UpdateVideoStreamAsync(UpdateVideoStreamRequest request, CancellationToken cancellationToken);

        IQueryable<VideoStream> GetVideoStreamsByMatchingIds(IEnumerable<string> VideoStreamIds);

        Task<PagedResponse<VideoStreamDto>> GetVideoStreams(VideoStreamParameters VideoStreamParameters, CancellationToken cancellationToken);

        IQueryable<VideoStream> GetAllVideoStreams();

        Task<VideoStream?> GetVideoStreamByIdAsync(string VideoStreamId, CancellationToken cancellationToken = default);

        Task<(VideoStreamHandlers videoStreamHandler, List<ChildVideoStreamDto> childVideoStreamDtos)?> GetStreamsFromVideoStreamById(string videoStreamId, CancellationToken cancellationToken = default);

        Task<VideoStreamDto?> GetVideoStreamDtoByIdAsync(string VideoStreamId, CancellationToken cancellationToken = default);
    }
}