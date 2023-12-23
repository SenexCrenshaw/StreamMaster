using StreamMasterDomain.Dto;
using StreamMasterDomain.Models;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Requests;

namespace StreamMasterDomain.Repository
{
    public interface IVideoStreamRepository : IRepositoryBase<VideoStream>
    {
        PagedResponse<VideoStreamDto> CreateEmptyPagedResponse();

        Task UpdateVideoStreamsChannelGroupNames(IEnumerable<string> videoStreamIds, string newName);

        Task<List<VideoStreamDto>> GetVideoStreamsForChannelGroups(IEnumerable<int> channelGroupIds, CancellationToken cancellationToken);

        Task<List<VideoStreamDto>> GetVideoStreamsForChannelGroup(int channelGroupId, CancellationToken cancellationToken);

        Task<VideoStream?> CreateVideoStreamAsync(CreateVideoStreamRequest request, CancellationToken cancellationToken);

        Task<VideoStreamDto?> GetVideoStreamById(string Id);

        Task<List<string>> GetVideoStreamNames();

        Task<List<VideoStreamDto>> DeleteVideoStreamsByM3UFiledId(int M3UFileId, CancellationToken cancellationToken);

        Task<VideoStreamDto?> DeleteVideoStreamById(string VideoStreamId, CancellationToken cancellationToken);

        //Task<string?> DeleteVideoStream(VideoStream VideoStream);

        Task<List<VideoStreamDto>> GetVideoStreamsNotHidden();

        Task<List<VideoStreamDto>> GetVideoStreamsByM3UFileId(int m3uFileId);

        Task<List<VideoStreamDto>> SetVideoStreamChannelGroupName(string channelGroupName, string newGroupName, CancellationToken cancellationToken);

        Task<List<VideoStreamDto>> SetVideoStreamsLogoFromEPGFromIds(IEnumerable<string> VideoStreamIds, CancellationToken cancellationToken);

        Task<List<VideoStreamDto>> ReSetVideoStreamsLogoFromIds(IEnumerable<string> VideoStreamIds, CancellationToken cancellationToken);

        Task<List<VideoStreamDto>> ReSetVideoStreamsLogoFromParameters(VideoStreamParameters Parameters, CancellationToken cancellationToken);

        Task<List<VideoStreamDto>> SetVideoStreamChannelNumbersFromIds(IEnumerable<string> VideoStreamIds, bool OverWriteExisting, int StartNumber, string OrderBy, CancellationToken cancellationToken);

        Task<List<VideoStreamDto>> SetVideoStreamChannelNumbersFromParameters(VideoStreamParameters Parameters, bool OverWriteExisting, int StartNumber, CancellationToken cancellationToken);

        Task<List<VideoStreamDto>> SetVideoStreamsLogoFromEPGFromParameters(VideoStreamParameters Parameters, CancellationToken cancellationToken);

        Task<List<VideoStreamDto>> SetVideoStreamSetEPGsFromName(IEnumerable<string> VideoStreamIds, CancellationToken cancellationToken);

        Task<List<string>> DeleteAllVideoStreamsFromParameters(VideoStreamParameters Parameters, CancellationToken cancellationToken);

        Task<(List<VideoStreamDto> videoStreams, bool updateChannelGroup)> UpdateAllVideoStreamsFromParameters(VideoStreamParameters Parameters, UpdateVideoStreamRequest request, CancellationToken cancellationToken);

        Task<(List<VideoStreamDto> videoStreams, List<ChannelGroupDto> updatedChannelGroups)> UpdateVideoStreamsAsync(IEnumerable<UpdateVideoStreamRequest> VideoStreamUpdates, CancellationToken cancellationToken);

        Task<(VideoStreamDto? videoStream, ChannelGroupDto? updatedChannelGroup)> UpdateVideoStreamAsync(UpdateVideoStreamRequest request, CancellationToken cancellationToken);

        Task<PagedResponse<VideoStreamDto>> GetPagedVideoStreams(VideoStreamParameters Parameters, CancellationToken cancellationToken);

        Task<List<VideoStreamDto>> GetVideoStreams();

        IQueryable<VideoStream> GetVideoStreamQuery();

        Task<(VideoStreamHandlers videoStreamHandler, List<VideoStreamDto> childVideoStreamDtos)?> GetStreamsFromVideoStreamById(string videoStreamId, CancellationToken cancellationToken = default);

        //Task<List<VideoStreamDto>> SetGroupVisibleByGroupId(int id, bool isHidden, CancellationToken cancellationToken);
        Task<List<VideoStreamDto>> SetGroupVisibleByGroupName(string channelGroupName, bool isHidden, CancellationToken cancellationToken);
        Task<List<VideoStreamDto>> AutoSetEPGFromIds(List<string> ids, CancellationToken cancellationToken);
        Task<List<VideoStreamDto>> AutoSetEPGFromParameters(VideoStreamParameters parameters, List<string> ids, CancellationToken cancellationToken);
        Task<List<VideoStreamDto>> SetVideoStreamTimeShiftsFromIds(List<string> ids, string timeShift, CancellationToken cancellationToken);
        Task<List<VideoStreamDto>> SetVideoStreamTimeShiftFromParameters(VideoStreamParameters parameters, string timeShift, CancellationToken cancellationToken);
    }
}