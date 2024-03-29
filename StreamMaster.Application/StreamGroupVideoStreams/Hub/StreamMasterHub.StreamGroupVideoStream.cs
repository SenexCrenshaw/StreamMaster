﻿using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Pagination;

using StreamMaster.Application.StreamGroupVideoStreams;
using StreamMaster.Application.StreamGroupVideoStreams.Commands;
using StreamMaster.Application.StreamGroupVideoStreams.Queries;

namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub : IStreamGroupVideoStreamHub
{
    public async Task<List<VideoStreamIsReadOnly>> GetStreamGroupVideoStreamIds(GetStreamGroupVideoStreamIdsRequest request, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<PagedResponse<VideoStreamDto>> GetPagedStreamGroupVideoStreams(StreamGroupVideoStreamParameters Parameters, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetPagedStreamGroupVideoStreams(Parameters), cancellationToken).ConfigureAwait(false);
    }

    public async Task SetVideoStreamRanks(SetVideoStreamRanksRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task SyncVideoStreamToStreamGroup(SyncVideoStreamToStreamGroupRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task SetStreamGroupVideoStreamChannelNumbers(SetStreamGroupVideoStreamChannelNumbersRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }
}