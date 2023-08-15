using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupNamesFromVideoStream(VideoStreamDto VideoStreamDto) : IRequest<List<string>>;

internal class GetChannelGroupNamesFromVideoStreamHandler : BaseMediatorRequestHandler, IRequestHandler<GetChannelGroupNamesFromVideoStream, List<string>>
{
    public GetChannelGroupNamesFromVideoStreamHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<List<string>> Handle(GetChannelGroupNamesFromVideoStream request, CancellationToken cancellationToken)
    {

        List<string> name = await Repository.ChannelGroup.GetChannelNamesFromVideoStream(request.VideoStreamDto, cancellationToken).ConfigureAwait(false);
        return name;
    }
}