using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetVideoStreamsForChannelGroups(VideoStreamParameters VideoStreamParameters) : IRequest<PagedResponse<VideoStreamDto>>;

internal class GetVideoStreamsForChannelGroupsHandler : BaseMediatorRequestHandler, IRequestHandler<GetVideoStreamsForChannelGroups, PagedResponse<VideoStreamDto>>
{
    public GetVideoStreamsForChannelGroupsHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<PagedResponse<VideoStreamDto>> Handle(GetVideoStreamsForChannelGroups request, CancellationToken cancellationToken)
    {
        PagedResponse<VideoStreamDto> ret = await Repository.VideoStream.GetVideoStreamsForChannelGroups(request.VideoStreamParameters, cancellationToken);
        return ret;
    }
}