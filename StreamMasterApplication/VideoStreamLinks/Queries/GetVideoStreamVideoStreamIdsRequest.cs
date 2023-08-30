using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterDomain.Dto;

using System.Diagnostics;

namespace StreamMasterApplication.VideoStreamLinks.Queries;

public record GetVideoStreamVideoStreamIdsRequest(string videoStreamId) : IRequest<List<string>>;

internal class GetVideoStreamVideoStreamIdsRequestHandler : BaseRequestHandler, IRequestHandler<GetVideoStreamVideoStreamIdsRequest, List<string>>
{
    public GetVideoStreamVideoStreamIdsRequestHandler(ILogger<GetVideoStreamVideoStreamIdsRequest> logger, IRepositoryWrapper repository, IMapper mapper)
        : base(logger, repository, mapper) { }

    public async Task<List<string>> Handle(GetVideoStreamVideoStreamIdsRequest request, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        var res = await Repository.VideoStream.GetVideoStreamVideoStreamIds(request.videoStreamId, cancellationToken).ConfigureAwait(false);
        stopwatch.Stop();
        Logger.LogInformation($"GetVideoStreamVideoStreamIdsRequest took {stopwatch.ElapsedMilliseconds} ms");
        return res;
    }
}