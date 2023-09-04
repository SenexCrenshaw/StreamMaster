using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.VideoStreams.Commands;

public record SetVideoStreamsLogoFromEPGFromParametersRequest(VideoStreamParameters Parameters) : IRequest { }

public class SetVideoStreamsLogoFromEPGFromParametersRequestHandler : BaseMediatorRequestHandler, IRequestHandler<SetVideoStreamsLogoFromEPGFromParametersRequest>
{

    public SetVideoStreamsLogoFromEPGFromParametersRequestHandler(ILogger<SetVideoStreamsLogoFromEPGFromParametersRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task Handle(SetVideoStreamsLogoFromEPGFromParametersRequest request, CancellationToken cancellationToken)
    {
        int count = await Repository.VideoStream.SetVideoStreamsLogoFromEPGFromParameters(request.Parameters, cancellationToken).ConfigureAwait(false);

        if (count > 0)
        {
            await Publisher.Publish(new UpdateVideoStreamEvent(), cancellationToken).ConfigureAwait(false);
        }
    }
}