using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.Common.Models;
using StreamMasterApplication.StreamGroups.Commands;
using StreamMasterApplication.VideoStreams.Commands;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams;

public interface IVideoStreamController
{
    Task<IActionResult> AddVideoStream(AddVideoStreamRequest request);

    Task<IActionResult> ChangeVideoStreamChannel(ChangeVideoStreamChannelRequest request);

    Task<IActionResult> DeleteVideoStream(DeleteVideoStreamRequest request);

    Task<IActionResult> FailClient(FailClientRequest request);

    Task<IActionResult> GetAllStatisticsForAllUrls();

    Task<IActionResult> GetChannelLogoDtos();

    Task<ActionResult<VideoStreamDto?>> GetVideoStream(string id);

    Task<ActionResult<List<VideoStreamDto>>> GetVideoStreams();

    Task<IActionResult> GetVideoStreamStream(string encodedId, string name, CancellationToken cancellationToken);

    Task<IActionResult> ReSetVideoStreamsLogo(ReSetVideoStreamsLogoRequest request);

    Task<IActionResult> SetVideoStreamChannelNumbers(SetVideoStreamChannelNumbersRequest request);

    Task<IActionResult> SetVideoStreamsLogoToEPG(SetVideoStreamsLogoToEPGRequest request);
    Task<IActionResult> SetVideoStreamSetEPGsFromName(SetVideoStreamSetEPGsFromNameRequest request);

    IActionResult SimulateStreamFailure(string streamUrl);

    Task<IActionResult> UpdateVideoStream(UpdateVideoStreamRequest request);

    Task<IActionResult> UpdateVideoStreams(UpdateVideoStreamsRequest request);
}

public interface IVideoStreamDB
{
    DbSet<VideoStreamLink> VideoStreamLinks { get; set; }

    DbSet<VideoStream> VideoStreams { get; set; }

    Task<bool> BuildIconsCacheFromVideoStreams(CancellationToken cancellationToken);

    Task<bool> CacheIconsFromVideoStreams(CancellationToken cancellationToken);

    Task<bool> DeleteVideoStreamAsync(string videoStreamId, CancellationToken cancellationToken);

    public Task<List<VideoStream>> DeleteVideoStreamsByM3UFiledId(int M3UFileId, CancellationToken cancellationToken);

    Task<List<VideoStream>> GetAllVideoStreamsWithChildrenAsync(CancellationToken cancellationToken);

    Task<string> GetAvailableID();

    public M3UFileIdMaxStream? GetM3UFileIdMaxStreamFromUrl(string Url);

    Task<(VideoStreamHandlers videoStreamHandler, List<ChildVideoStreamDto> childVideoStreamDtos)?> GetStreamsFromVideoStreamById(string videoStreamId, CancellationToken cancellationToken = default);

    Task<VideoStreamDto> GetVideoStreamDto(string videoStreamId, CancellationToken cancellationToken);

    Task<List<VideoStreamDto>> GetVideoStreamsDto(CancellationToken cancellationToken);

    Task<List<VideoStream>> GetVideoStreamsForParentAsync(string parentVideoStreamId, CancellationToken cancellationToken);

    Task<bool> SynchronizeChildRelationships(VideoStream videoStream, List<ChildVideoStreamDto> videoStreamDtos, CancellationToken cancellationToken);

    Task<VideoStreamDto?> UpdateVideoStreamAsync(UpdateVideoStreamRequest request, CancellationToken cancellationToken);
}

public interface IVideoStreamHub
{
    Task AddVideoStream(AddVideoStreamRequest request);

    Task ChangeVideoStreamChannel(ChangeVideoStreamChannelRequest request);

    Task DeleteVideoStream(DeleteVideoStreamRequest request);

    Task<IEnumerable<ChannelLogoDto>> GetChannelLogoDtos();
    
    Task<VideoStreamDto?> GetVideoStream(string id);

    Task<IEnumerable<VideoStreamDto>> GetVideoStreams();

    Task ReSetVideoStreamsLogo(ReSetVideoStreamsLogoRequest request);

    Task SetVideoStreamChannelNumbers(SetVideoStreamChannelNumbersRequest request);

    Task SetVideoStreamsLogoToEPG(SetVideoStreamsLogoToEPGRequest request);
    Task SetVideoStreamSetEPGsFromName(SetVideoStreamSetEPGsFromNameRequest request);

    Task UpdateVideoStream(UpdateVideoStreamRequest request);

    Task UpdateVideoStreams(UpdateVideoStreamsRequest request);
}

public interface IVideoStreamScoped
{
}

public interface IVideoStreamTasks
{
}
