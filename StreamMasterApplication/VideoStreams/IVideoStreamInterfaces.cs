using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.Common.Models;
using StreamMasterApplication.VideoStreams.Commands;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams;

public interface IVideoStreamController
{
    Task<ActionResult> AddVideoStream(AddVideoStreamRequest request);

    Task<ActionResult> DeleteVideoStream(DeleteVideoStreamRequest request);

    Task<ActionResult<VideoStreamDto?>> GetVideoStream(int id);

    Task<ActionResult<List<VideoStreamDto>>> GetVideoStreams();

    Task<ActionResult> SetVideoStreamChannelNumbers(SetVideoStreamChannelNumbersRequest request);

    Task<ActionResult> UpdateVideoStream(UpdateVideoStreamRequest request);

    Task<ActionResult> UpdateVideoStreams(UpdateVideoStreamsRequest request);
}

public interface IVideoStreamDB
{
    DbSet<VideoStreamRelationship> VideoStreamRelationships { get; set; }

    DbSet<VideoStream> VideoStreams { get; set; }

    public bool SynchronizeChildRelationships(VideoStream videoStream, List<ChildVideoStreamDto> videoStreamDtos);

    public Task<bool> DeleteVideoStream(int VideoStreamId);
}

public interface IVideoStreamHub
{
    Task<VideoStreamDto?> AddVideoStream(AddVideoStreamRequest request);

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