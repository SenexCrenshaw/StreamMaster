using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

using System.Diagnostics;

namespace StreamMasterApplication.VideoStreamLinks.Queries;

public record GetVideoStreamVideoStreamsRequest(VideoStreamLinkParameters Parameters) : IRequest<PagedResponse<ChildVideoStreamDto>>;

internal class GetVideoStreamVideoStreamsRequestHandler : BaseRequestHandler, IRequestHandler<GetVideoStreamVideoStreamsRequest, PagedResponse<ChildVideoStreamDto>>
{
    public GetVideoStreamVideoStreamsRequestHandler(ILogger<GetVideoStreamVideoStreamsRequest> logger, IRepositoryWrapper repository, IMapper mapper)
        : base(logger, repository, mapper) { }

    public async Task<PagedResponse<ChildVideoStreamDto>> Handle(GetVideoStreamVideoStreamsRequest request, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        var res = await Repository.VideoStreamLink.GetVideoStreamVideoStreams(request.Parameters, cancellationToken).ConfigureAwait(false);
        stopwatch.Stop();
        Logger.LogInformation($"GetVideoStreamVideoStreamsRequest took {stopwatch.ElapsedMilliseconds} ms");
        return res;
    }
}