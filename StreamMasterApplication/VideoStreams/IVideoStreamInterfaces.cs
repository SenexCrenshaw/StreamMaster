using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.Common.Models;
using StreamMasterApplication.VideoStreams.Commands;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams;

public interface IVideoStreamController
{
    Task<ActionResult> AddVideoStream(AddVideoStreamRequest request);

    Task<ActionResult> ChangeVideoStreamChannel(ChangeVideoStreamChannelRequest request);

    Task<ActionResult> DeleteVideoStream(DeleteVideoStreamRequest request);

    Task<ActionResult<VideoStreamDto?>> GetVideoStream(int id);

    Task<ActionResult<List<VideoStreamDto>>> GetVideoStreams();

    Task<ActionResult> SetVideoStreamChannelNumbers(SetVideoStreamChannelNumbersRequest request);

    Task<ActionResult> UpdateVideoStream(UpdateVideoStreamRequest request);

    Task<ActionResult> UpdateVideoStreams(UpdateVideoStreamsRequest request);
}

public interface IVideoStreamDB
{
    DbSet<VideoStreamLink> VideoStreamLinks { get; set; }
    DbSet<VideoStream> VideoStreams { get; set; }

    Task<bool> DeleteVideoStreamAsync(int videoStreamId, CancellationToken cancellationToken);

    public Task<List<VideoStream>> DeleteVideoStreamsByM3UFiledId(int M3UFileId, CancellationToken cancellationToken);

    Task<List<VideoStream>> GetAllVideoStreamsWithChildrenAsync();

    Task<List<VideoStream>> GetChildVideoStreamsAsync(int parentId);

    public M3UFileIdMaxStream? GetM3UFileIdMaxStreamFromUrl(string Url);

    Task<(VideoStreamHandlers videoStreamHandler, List<ChildVideoStreamDto> childVideoStreamDtos)?> GetStreamsFromVideoStreamById(int videoStreamId, CancellationToken cancellationToken = default);

    //Task<VideoStreamDto?> GetVideoStream(int videoStreamId, CancellationToken cancellationToken = default);

    Task<VideoStreamDto> GetVideoStreamDtoWithChildrenAsync(int videoStreamId, CancellationToken cancellationToken);

    Task<VideoStream> GetVideoStreamWithChildrenAsync(int videoStreamId, CancellationToken cancellationToken);

    Task<bool> SynchronizeChildRelationships(VideoStream videoStream, List<ChildVideoStreamDto> videoStreamDtos, CancellationToken cancellationToken);

    Task<VideoStreamDto?> UpdateVideoStreamAsync(UpdateVideoStreamRequest request, CancellationToken cancellationToken);
}

public interface IVideoStreamHub
{
    Task<VideoStreamDto?> AddVideoStream(AddVideoStreamRequest request);

    Task ChangeVideoStreamChannel(ChangeVideoStreamChannelRequest request);

    Task<int?> DeleteVideoStream(DeleteVideoStreamRequest request);

    Task<VideoStreamDto?> GetVideoStream(int id);

    Task<IEnumerable<VideoStreamDto>> GetVideoStreams();

    Task<IEnumerable<ChannelNumberPair>> SetVideoStreamChannelNumbers(SetVideoStreamChannelNumbersRequest request);

    Task<VideoStreamDto?> UpdateVideoStream(UpdateVideoStreamRequest request);

    Task<IEnumerable<VideoStreamDto>> UpdateVideoStreams(UpdateVideoStreamsRequest request);
}

public interface IVideoStreamScoped
{
}

public interface IVideoStreamTasks
{
}
