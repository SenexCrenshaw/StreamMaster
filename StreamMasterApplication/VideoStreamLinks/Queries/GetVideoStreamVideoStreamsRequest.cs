using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterDomain.Dto;

using System.Diagnostics;

namespace StreamMasterApplication.VideoStreamLinks.Queries;

public record GetVideoStreamVideoStreamsRequest(string videoStreamId) : IRequest<List<ChildVideoStreamDto>>;

internal class GetVideoStreamVideoStreamsRequestHandler : BaseRequestHandler, IRequestHandler<GetVideoStreamVideoStreamsRequest, List<ChildVideoStreamDto>>
{
    public GetVideoStreamVideoStreamsRequestHandler(ILogger<GetVideoStreamVideoStreamsRequest> logger, IRepositoryWrapper repository, IMapper mapper)
        : base(logger, repository, mapper) { }

    public async Task<List<ChildVideoStreamDto>> Handle(GetVideoStreamVideoStreamsRequest request, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        var res = await Repository.VideoStreamLink.GetVideoStreamVideoStreams(request.videoStreamId, cancellationToken).ConfigureAwait(false);
        stopwatch.Stop();
        Logger.LogInformation($"GetVideoStreamVideoStreamsRequest took {stopwatch.ElapsedMilliseconds} ms");
        return res;
    }
}