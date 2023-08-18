using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupIdsFromVideoStream(VideoStreamDto VideoStreamDto) : IRequest<List<int>>;

internal class GetChannelGroupIdsFromVideoStreamHandler : BaseMediatorRequestHandler, IRequestHandler<GetChannelGroupIdsFromVideoStream, List<int>>
{
    public GetChannelGroupIdsFromVideoStreamHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<List<int>> Handle(GetChannelGroupIdsFromVideoStream request, CancellationToken cancellationToken)
    {

        List<int> ids = await Repository.ChannelGroup.GetChannelIdsFromVideoStream(request.VideoStreamDto, cancellationToken).ConfigureAwait(false);
        return ids;
    }
}