using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

using System.Diagnostics;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetVideoStreamsForChannelGroups(VideoStreamParameters VideoStreamParameters) : IRequest<PagedResponse<VideoStreamDto>>;

internal class GetVideoStreamsForChannelGroupsHandler : BaseMediatorRequestHandler, IRequestHandler<GetVideoStreamsForChannelGroups, PagedResponse<VideoStreamDto>>
{
    public GetVideoStreamsForChannelGroupsHandler(ILogger<GetVideoStreamsForChannelGroups> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<PagedResponse<VideoStreamDto>> Handle(GetVideoStreamsForChannelGroups request, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        PagedResponse<VideoStreamDto> ret = await Repository.VideoStream.GetVideoStreams(request.VideoStreamParameters, cancellationToken);
        stopwatch.Stop();
        Logger.LogInformation($"ElapsedMilliseconds: {stopwatch.ElapsedMilliseconds} ms got {ret.Data.Count} out of {ret.TotalItemCount}");

        return ret;
    }
}