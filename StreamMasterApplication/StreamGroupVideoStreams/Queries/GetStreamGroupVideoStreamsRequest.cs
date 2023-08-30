using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterDomain.Dto;

using System.Diagnostics;

namespace StreamMasterApplication.StreamGroupVideoStreams.Queries;

public record GetStreamGroupVideoStreamsRequest(int streamGroupId) : IRequest<List<VideoStreamDto>>;

internal class GetStreamGroupVideoStreamsRequestHandler : BaseRequestHandler, IRequestHandler<GetStreamGroupVideoStreamsRequest, List<VideoStreamDto>>
{
    public GetStreamGroupVideoStreamsRequestHandler(ILogger<GetStreamGroupVideoStreamsRequest> logger, IRepositoryWrapper repository, IMapper mapper)
        : base(logger, repository, mapper) { }

    public async Task<List<VideoStreamDto>> Handle(GetStreamGroupVideoStreamsRequest request, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        List<VideoStreamDto> res = await Repository.StreamGroupVideoStream.GetStreamGroupVideoStreams(request.streamGroupId, cancellationToken).ConfigureAwait(false);
        stopwatch.Stop();
        Logger.LogInformation($"GetStreamGroupVideoStreamsRequest took {stopwatch.ElapsedMilliseconds} ms");
        return res;
    }
}