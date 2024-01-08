using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.StreamGroups.Commands;
using StreamMaster.Application.VideoStreams.Commands;
using StreamMaster.Application.VideoStreams.Queries;
using StreamMaster.Domain.Pagination;
using StreamMaster.Domain.Requests;


namespace StreamMaster.Application.VideoStreams;

public interface IVideoStreamController
{
    Task<ActionResult<VideoInfo>> GetVideoStreamInfoFromId(GetVideoStreamInfoFromIdRequest request);
    Task<ActionResult<VideoInfo>> GetVideoStreamInfoFromUrl(GetVideoStreamInfoFromUrlRequest request);

    Task<IActionResult> AutoSetEPG(AutoSetEPGRequest request);
    Task<IActionResult> AutoSetEPGFromParameters(AutoSetEPGFromParametersRequest request);
    Task<IActionResult> SimulateStreamFailure(SimulateStreamFailureRequest request);
    Task<ActionResult> DeleteAllVideoStreamsFromParameters(DeleteAllVideoStreamsFromParametersRequest request);
    Task<ActionResult> UpdateAllVideoStreamsFromParameters(UpdateAllVideoStreamsFromParametersRequest request);

    Task<ActionResult> CreateVideoStream(CreateVideoStreamRequest request);

    Task<ActionResult> ChangeVideoStreamChannel(ChangeVideoStreamChannelRequest request);

    Task<ActionResult> DeleteVideoStream(DeleteVideoStreamRequest request);

    IActionResult FailClient(FailClientRequest request);

    Task<ActionResult<VideoStreamDto?>> GetVideoStream(string id);

    Task<ActionResult<List<IdName>>> GetVideoStreamNames();

    Task<ActionResult<PagedResponse<VideoStreamDto>>> GetPagedVideoStreams([FromQuery] VideoStreamParameters Parameters);

    Task<ActionResult> GetVideoStreamStream(string encodedId, string name, CancellationToken cancellationToken);

    Task<ActionResult> ReSetVideoStreamsLogo(ReSetVideoStreamsLogoRequest request);
    Task<ActionResult> ReSetVideoStreamsLogoFromParameters(ReSetVideoStreamsLogoFromParametersRequest request);

    Task<ActionResult> SetVideoStreamsLogoFromEPG(SetVideoStreamsLogoFromEPGRequest request);
    Task<ActionResult> SetVideoStreamsLogoFromEPGFromParameters(SetVideoStreamsLogoFromEPGFromParametersRequest request);

    Task<ActionResult> SetVideoStreamTimeShifts(SetVideoStreamTimeShiftsRequest request);
    Task<ActionResult> SetVideoStreamTimeShiftFromParameters(SetVideoStreamTimeShiftFromParametersRequest request);

    Task<ActionResult> SetVideoStreamChannelNumbersFromParameters(SetVideoStreamChannelNumbersFromParametersRequest request);
    Task<ActionResult> SetVideoStreamChannelNumbers(SetVideoStreamChannelNumbersRequest request);



    Task<ActionResult> UpdateVideoStream(UpdateVideoStreamRequest request);

    Task<ActionResult> UpdateVideoStreams(UpdateVideoStreamsRequest request);
}

public interface IVideoStreamHub
{
    Task<VideoInfo> GetVideoStreamInfoFromId(string channelVideoStreamId);
    Task<VideoInfo> GetVideoStreamInfoFromUrl(string channelVideoStreamId);

    Task AutoSetEPGFromParameters(AutoSetEPGFromParametersRequest request);
    Task AutoSetEPG(AutoSetEPGRequest request);
    Task SimulateStreamFailure(SimulateStreamFailureRequest request);
    Task DeleteAllVideoStreamsFromParameters(DeleteAllVideoStreamsFromParametersRequest request);
    Task UpdateAllVideoStreamsFromParameters(UpdateAllVideoStreamsFromParametersRequest request);
    Task<List<IdName>> GetVideoStreamNames();
    Task CreateVideoStream(CreateVideoStreamRequest request);

    Task ChangeVideoStreamChannel(ChangeVideoStreamChannelRequest request);

    Task DeleteVideoStream(DeleteVideoStreamRequest request);

    Task<VideoStreamDto?> GetVideoStream(string id);

    Task<PagedResponse<VideoStreamDto>> GetPagedVideoStreams(VideoStreamParameters Parameters);
    Task SetVideoStreamsLogoFromEPGFromParameters(SetVideoStreamsLogoFromEPGFromParametersRequest request);
    Task ReSetVideoStreamsLogo(ReSetVideoStreamsLogoRequest request);
    Task ReSetVideoStreamsLogoFromParameters(ReSetVideoStreamsLogoFromParametersRequest request);
    Task SetVideoStreamChannelNumbersFromParameters(SetVideoStreamChannelNumbersFromParametersRequest request);
    Task SetVideoStreamChannelNumbers(SetVideoStreamChannelNumbersRequest request);

    Task SetVideoStreamTimeShifts(SetVideoStreamTimeShiftsRequest request);
    Task SetVideoStreamTimeShiftFromParameters(SetVideoStreamTimeShiftFromParametersRequest request);

    Task SetVideoStreamsLogoFromEPG(SetVideoStreamsLogoFromEPGRequest request);

    Task UpdateVideoStream(UpdateVideoStreamRequest request);

    Task UpdateVideoStreams(UpdateVideoStreamsRequest request);
}

public interface IVideoStreamScoped
{
}

public interface IVideoStreamTasks
{
}