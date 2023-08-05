using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.StreamGroups.Commands;
using StreamMasterApplication.VideoStreams.Commands;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.VideoStreams;

public interface IVideoStreamController
{
    Task<ActionResult> CreateVideoStream(CreateVideoStreamRequest request);

    Task<ActionResult> ChangeVideoStreamChannel(ChangeVideoStreamChannelRequest request);

    Task<ActionResult> DeleteVideoStream(DeleteVideoStreamRequest request);

    Task<ActionResult> FailClient(FailClientRequest request);

    Task<ActionResult> GetAllStatisticsForAllUrls();

    Task<ActionResult> GetChannelLogoDtos();

    Task<ActionResult<VideoStreamDto?>> GetVideoStream(string id);

    Task<ActionResult<IEnumerable<VideoStreamDto>>> GetVideoStreams(VideoStreamParameters Parameters);

    Task<ActionResult> GetVideoStreamStream(string encodedId, string name, CancellationToken cancellationToken);

    Task<ActionResult> ReSetVideoStreamsLogo(ReSetVideoStreamsLogoRequest request);

    Task<ActionResult> SetVideoStreamChannelNumbers(SetVideoStreamChannelNumbersRequest request);

    Task<ActionResult> SetVideoStreamSetEPGsFromName(SetVideoStreamSetEPGsFromNameRequest request);

    Task<ActionResult> SetVideoStreamsLogoToEPG(SetVideoStreamsLogoToEPGRequest request);

    ActionResult SimulateStreamFailure(string streamUrl);

    Task<ActionResult> UpdateVideoStream(UpdateVideoStreamRequest request);

    Task<ActionResult> UpdateVideoStreams(UpdateVideoStreamsRequest request);
}

public interface IVideoStreamDB
{


    //Task<bool> BuildIconsCacheFromVideoStreams(CancellationToken cancellationToken);

    //Task<bool> CacheIconsFromVideoStreams(CancellationToken cancellationToken);

    //Task<bool> DeleteVideoStreamAsync(string videoStreamId, CancellationToken cancellationToken);

    //Task<int> DeleteVideoStreamsAsync(List<VideoStream> videoStreams, CancellationToken cancellationToken);

    //public Task<List<VideoStream>> DeleteVideoStreamsByM3UFiledId(int M3UFileId, CancellationToken cancellationToken);

    //Task<List<VideoStream>> GetAllVideoStreamsWithChildrenAsync(CancellationToken cancellationToken);

    //Task<string> GetAvailableID();

    //Task<M3UFileIdMaxStream?> GetM3UFileIdMaxStreamFromUrl(string Url);

    //Task<(VideoStreamHandlers videoStreamHandler, List<ChildVideoStreamDto> childVideoStreamDtos)?> GetStreamsFromVideoStreamById(string videoStreamId, CancellationToken cancellationToken = default);

    //Task<VideoStreamDto> GetVideoStreamDto(string videoStreamId, CancellationToken cancellationToken);

    //Task<List<VideoStreamDto>> GetVideoStreamsDto(CancellationToken cancellationToken);

    //Task<List<VideoStream>> GetVideoStreamsForParentAsync(string parentVideoStreamId, CancellationToken cancellationToken);

    ////Task<bool> SynchronizeChildRelationships(VideoStream videoStream, List<ChildVideoStreamDto> videoStreamDtos, CancellationToken cancellationToken);

    //Task<VideoStreamDto?> UpdateVideoStreamAsync(UpdateVideoStreamRequest request, CancellationToken cancellationToken);
}

public interface IVideoStreamHub
{
    Task CreateVideoStream(CreateVideoStreamRequest request);

    Task ChangeVideoStreamChannel(ChangeVideoStreamChannelRequest request);

    Task DeleteVideoStream(DeleteVideoStreamRequest request);

    Task<IEnumerable<ChannelLogoDto>> GetChannelLogoDtos();

    Task<VideoStreamDto?> GetVideoStream(string id);

    Task<PagedList<VideoStream>> GetVideoStreams(VideoStreamParameters Parameters);

    Task ReSetVideoStreamsLogo(ReSetVideoStreamsLogoRequest request);

    Task SetVideoStreamChannelNumbers(SetVideoStreamChannelNumbersRequest request);

    Task SetVideoStreamSetEPGsFromName(SetVideoStreamSetEPGsFromNameRequest request);

    Task SetVideoStreamsLogoToEPG(SetVideoStreamsLogoToEPGRequest request);

    Task UpdateVideoStream(UpdateVideoStreamRequest request);

    Task UpdateVideoStreams(UpdateVideoStreamsRequest request);
}

public interface IVideoStreamScoped
{
}

public interface IVideoStreamTasks
{
}
