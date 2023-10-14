using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.StreamGroupVideoStreams.Commands;
using StreamMasterApplication.StreamGroupVideoStreams.Queries;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.StreamGroupVideoStreams;

public interface IStreamGroupVideoStreamController
{
    Task<IActionResult> SetStreamGroupVideoStreamChannelNumbers(SetStreamGroupVideoStreamChannelNumbersRequest request, CancellationToken cancellationToken);
    Task<IActionResult> SetVideoStreamRanks(SetVideoStreamRanksRequest request, CancellationToken cancellationToken);
    Task<ActionResult<PagedResponse<VideoStreamDto>>> GetPagedStreamGroupVideoStreams(StreamGroupVideoStreamParameters Parameters, CancellationToken cancellationToken = default);

    Task<ActionResult<List<VideoStreamIsReadOnly>>> GetStreamGroupVideoStreamIds(GetStreamGroupVideoStreamIdsRequest request, CancellationToken cancellationToken = default);

    Task<IActionResult> SyncVideoStreamToStreamGroup(SyncVideoStreamToStreamGroupRequest request, CancellationToken cancellationToken);
    //Task<IActionResult> AddVideoStreamToStreamGroup(AddVideoStreamToStreamGroupRequest request, CancellationToken cancellationToken);

    //Task<IActionResult> RemoveVideoStreamFromStreamGroup(RemoveVideoStreamFromStreamGroupRequest request, CancellationToken cancellationToken);
}

public interface IStreamGroupVideoStreamHub
{
    Task SetStreamGroupVideoStreamChannelNumbers(SetStreamGroupVideoStreamChannelNumbersRequest request);
    Task SetVideoStreamRanks(SetVideoStreamRanksRequest request, CancellationToken cancellationToken);
    Task SyncVideoStreamToStreamGroup(SyncVideoStreamToStreamGroupRequest request, CancellationToken cancellationToken);

    Task<PagedResponse<VideoStreamDto>> GetPagedStreamGroupVideoStreams(StreamGroupVideoStreamParameters Parameters, CancellationToken cancellationToken = default);

    Task<List<VideoStreamIsReadOnly>> GetStreamGroupVideoStreamIds(GetStreamGroupVideoStreamIdsRequest request, CancellationToken cancellationToken = default);
}