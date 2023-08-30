using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

using System.Diagnostics;

namespace StreamMasterApplication.StreamGroupVideoStreams.Queries;

public record GetStreamGroupVideoStreamsRequest(StreamGroupVideoStreamParameters Parameters) : IRequest<PagedResponse<VideoStreamDto>>;

internal class GetStreamGroupVideoStreamsRequestHandler : BaseRequestHandler, IRequestHandler<GetStreamGroupVideoStreamsRequest, PagedResponse<VideoStreamDto>>
{
    public GetStreamGroupVideoStreamsRequestHandler(ILogger<GetStreamGroupVideoStreamsRequest> logger, IRepositoryWrapper repository, IMapper mapper)
        : base(logger, repository, mapper) { }

    public async Task<PagedResponse<VideoStreamDto>> Handle(GetStreamGroupVideoStreamsRequest request, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        var res = await Repository.StreamGroupVideoStream.GetStreamGroupVideoStreams(request.Parameters, cancellationToken).ConfigureAwait(false);
        stopwatch.Stop();
        Logger.LogInformation($"GetStreamGroupVideoStreamsRequest took {stopwatch.ElapsedMilliseconds} ms");
        return res;
    }
}