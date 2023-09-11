﻿using StreamMasterApplication.VideoStreamLinks;
using StreamMasterApplication.VideoStreamLinks.Commands;
using StreamMasterApplication.VideoStreamLinks.Queries;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IVideoStreamLinkHub
{
    public async Task AddVideoStreamToVideoStream(AddVideoStreamToVideoStreamRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
    }

    public async Task<List<string>> GetVideoStreamVideoStreamIds(GetVideoStreamVideoStreamIdsRequest request, CancellationToken cancellationToken)
    {
        return await mediator.Send(request, cancellationToken);
    }

    public async Task<PagedResponse<ChildVideoStreamDto>> GetVideoStreamVideoStreams(VideoStreamLinkParameters Parameters, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetVideoStreamVideoStreamsRequest(Parameters), cancellationToken);
    }

    public async Task RemoveVideoStreamFromVideoStream(RemoveVideoStreamFromVideoStreamRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
    }
}