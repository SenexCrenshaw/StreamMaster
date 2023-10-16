using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.Common.Models;
using StreamMasterApplication.StreamGroups.Commands;
using StreamMasterApplication.VideoStreams.Commands;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.VideoStreams;

public interface IVideoStreamController
{
    Task<IActionResult> AutoSetEPG(AutoSetEPGRequest request);
    Task<IActionResult> AutoSetEPGFromParameters(AutoSetEPGFromParametersRequest request);
    Task<IActionResult> SimulateStreamFailure(SimulateStreamFailureRequest request);
    Task<ActionResult> DeleteAllVideoStreamsFromParameters(DeleteAllVideoStreamsFromParametersRequest request);
    Task<ActionResult> UpdateAllVideoStreamsFromParameters(UpdateAllVideoStreamsFromParametersRequest request);

    Task<ActionResult> CreateVideoStream(CreateVideoStreamRequest request);

    Task<ActionResult> ChangeVideoStreamChannel(ChangeVideoStreamChannelRequest request);

    Task<ActionResult> DeleteVideoStream(DeleteVideoStreamRequest request);

    IActionResult FailClient(FailClientRequest request);

    Task<ActionResult<List<StreamStatisticsResult>>> GetAllStatisticsForAllUrls();

    Task<ActionResult<List<ChannelLogoDto>>> GetChannelLogoDtos();

    Task<ActionResult<VideoStreamDto?>> GetVideoStream(string id);

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
    Task AutoSetEPGFromParameters(AutoSetEPGFromParametersRequest request);
    Task AutoSetEPG(AutoSetEPGRequest request);
    Task SimulateStreamFailure(SimulateStreamFailureRequest request);
    Task DeleteAllVideoStreamsFromParameters(DeleteAllVideoStreamsFromParametersRequest request);
    Task UpdateAllVideoStreamsFromParameters(UpdateAllVideoStreamsFromParametersRequest request);

    Task CreateVideoStream(CreateVideoStreamRequest request);

    Task ChangeVideoStreamChannel(ChangeVideoStreamChannelRequest request);

    Task DeleteVideoStream(DeleteVideoStreamRequest request);

    Task<IEnumerable<ChannelLogoDto>> GetChannelLogoDtos();

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