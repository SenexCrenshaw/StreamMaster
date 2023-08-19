using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

using System.Diagnostics;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetVideoStreams(VideoStreamParameters Parameters) : IRequest<PagedResponse<VideoStreamDto>>;

internal class GetVideoStreamsHandler : BaseRequestHandler, IRequestHandler<GetVideoStreams, PagedResponse<VideoStreamDto>>
{
    public GetVideoStreamsHandler(ILogger<GetVideoStreamsHandler> logger, IRepositoryWrapper repository, IMapper mapper)
        : base(logger, repository, mapper) { }

    public async Task<PagedResponse<VideoStreamDto>> Handle(GetVideoStreams request, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        PagedResponse<VideoStreamDto> res = await Repository.VideoStream.GetVideoStreams(request.Parameters, cancellationToken).ConfigureAwait(false);
        stopwatch.Stop();
        Logger.LogInformation($"GetVideoStreamsHandler took {stopwatch.ElapsedMilliseconds} ms");
        return res;


    }
}