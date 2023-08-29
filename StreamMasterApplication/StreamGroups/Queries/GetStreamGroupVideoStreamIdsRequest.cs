using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterDomain.Dto;

using System.Diagnostics;

namespace StreamMasterApplication.StreamGroups.Queries;

public record GetStreamGroupVideoStreamIdsRequest(int streamGroupId) : IRequest<List<VideoStreamIsReadOnly>>;

internal class GetStreamGroupVideoStreamIdsRequestHandler : BaseRequestHandler, IRequestHandler<GetStreamGroupVideoStreamIdsRequest, List<VideoStreamIsReadOnly>>
{
    public GetStreamGroupVideoStreamIdsRequestHandler(ILogger<GetStreamGroupVideoStreamIdsRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper)
        : base(logger, repository, mapper) { }

    public async Task<List<VideoStreamIsReadOnly>> Handle(GetStreamGroupVideoStreamIdsRequest request, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        List<VideoStreamIsReadOnly> res = await Repository.StreamGroup.GetStreamGroupVideoStreamIds(request.streamGroupId, cancellationToken).ConfigureAwait(false);
        stopwatch.Stop();
        Logger.LogInformation($"GetStreamGroupVideoStreamIdsRequestHandler took {stopwatch.ElapsedMilliseconds} ms");
        return res;
    }
}