﻿namespace StreamMaster.Application.VideoStreamLinks.Queries;

public record GetVideoStreamVideoStreamIdsRequest(string videoStreamId) : IRequest<List<string>>;

internal class GetVideoStreamVideoStreamIdsRequestHandler(ILogger<GetVideoStreamVideoStreamIdsRequest> logger, IRepositoryWrapper Repository) : IRequestHandler<GetVideoStreamVideoStreamIdsRequest, List<string>>
{
    public async Task<List<string>> Handle(GetVideoStreamVideoStreamIdsRequest request, CancellationToken cancellationToken)
    {

        return await Repository.VideoStreamLink.GetVideoStreamVideoStreamIds(request.videoStreamId, cancellationToken).ConfigureAwait(false);

    }
}