using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.Commands;

public record SetVideoStreamsLogoFromEPGRequest(List<string> Ids, string? OrderBy) : IRequest { }

public class SetVideoStreamsLogoFromEPGRequestHandler : BaseMemoryRequestHandler, IRequestHandler<SetVideoStreamsLogoFromEPGRequest>
{

    public SetVideoStreamsLogoFromEPGRequestHandler(ILogger<SetVideoStreamsLogoFromEPGRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public async Task Handle(SetVideoStreamsLogoFromEPGRequest request, CancellationToken cancellationToken)
    {
        int count = await Repository.VideoStream.SetVideoStreamsLogoFromEPGFromIds(request.Ids, cancellationToken).ConfigureAwait(false);

        if (count > 0)
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(), cancellationToken).ConfigureAwait(false);
        }
    }
}
