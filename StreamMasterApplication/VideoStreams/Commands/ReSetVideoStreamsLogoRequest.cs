using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.Commands;

public class ReSetVideoStreamsLogoRequest : IRequest
{
    public List<string> Ids { get; set; } = new List<string>();
}

public class ReSetVideoStreamsLogoHandler : BaseMediatorRequestHandler, IRequestHandler<ReSetVideoStreamsLogoRequest>
{

    public ReSetVideoStreamsLogoHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task Handle(ReSetVideoStreamsLogoRequest request, CancellationToken cancellationToken)
    {
        int count = await Repository.VideoStream.ReSetVideoStreamsLogoFromIds(request.Ids, cancellationToken).ConfigureAwait(false);

        if (count > 0)
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(), cancellationToken).ConfigureAwait(false);
        }
    }
}
