using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.Common.Models;
using StreamMasterApplication.StreamGroups.Commands;
using StreamMasterApplication.VideoStreamLinks.Queries;
using StreamMasterApplication.VideoStreams.Commands;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.VideoStreams;

public interface IVideoStreamController
{
    Task<ActionResult> UpdateAllVideoStreamsFromParameters(UpdateAllVideoStreamsFromParametersRequest request);

    Task<ActionResult> CreateVideoStream(CreateVideoStreamRequest request);

    Task<ActionResult> ChangeVideoStreamChannel(ChangeVideoStreamChannelRequest request);

    Task<ActionResult> DeleteVideoStream(DeleteVideoStreamRequest request);

    Task<ActionResult> FailClient(FailClientRequest request);

    Task<ActionResult<List<StreamStatisticsResult>>> GetAllStatisticsForAllUrls();

    Task<ActionResult<List<ChannelLogoDto>>> GetChannelLogoDtos();

    Task<ActionResult<VideoStreamDto?>> GetVideoStream(string id);

    Task<ActionResult<PagedResponse<VideoStreamDto>>> GetVideoStreams([FromQuery] VideoStreamParameters Parameters);

    Task<ActionResult> GetVideoStreamStream(string encodedId, string name, CancellationToken cancellationToken);

    Task<ActionResult> ReSetVideoStreamsLogo(ReSetVideoStreamsLogoRequest request);

    Task<ActionResult> SetVideoStreamChannelNumbers(SetVideoStreamChannelNumbersRequest request);

    Task<ActionResult> SetVideoStreamSetEPGsFromName(SetVideoStreamSetEPGsFromNameRequest request);

    Task<ActionResult> SetVideoStreamsLogoToEPG(SetVideoStreamsLogoToEPGRequest request);

    ActionResult SimulateStreamFailure(string streamUrl);

    Task<ActionResult> UpdateVideoStream(UpdateVideoStreamRequest request);

    Task<ActionResult> UpdateVideoStreams(UpdateVideoStreamsRequest request);
}

public interface IVideoStreamHub
{
    Task UpdateAllVideoStreamsFromParameters(UpdateAllVideoStreamsFromParametersRequest request);

    Task CreateVideoStream(CreateVideoStreamRequest request);

    Task ChangeVideoStreamChannel(ChangeVideoStreamChannelRequest request);

    Task DeleteVideoStream(DeleteVideoStreamRequest request);

    Task<IEnumerable<ChannelLogoDto>> GetChannelLogoDtos();

    Task<VideoStreamDto?> GetVideoStream(string id);

    Task<PagedResponse<VideoStreamDto>> GetVideoStreams(VideoStreamParameters Parameters);

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