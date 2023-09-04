using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.VideoStreams.Commands;

public record ReSetVideoStreamsLogoFromParametersRequest(VideoStreamParameters Parameters) : IRequest { }

public class ReSetVideoStreamsLogoFromParametersHandler : BaseMediatorRequestHandler, IRequestHandler<ReSetVideoStreamsLogoFromParametersRequest>
{

    public ReSetVideoStreamsLogoFromParametersHandler(ILogger<ReSetVideoStreamsLogoFromParametersRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task Handle(ReSetVideoStreamsLogoFromParametersRequest request, CancellationToken cancellationToken)
    {
        int count = await Repository.VideoStream.ReSetVideoStreamsLogoFromParameters(request.Parameters, cancellationToken).ConfigureAwait(false);

        if (count > 0)
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(), cancellationToken).ConfigureAwait(false);
        }
    }
}
