using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.ChannelGroups.Events;
using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.Commands;

public record UpdateVideoStreamsRequest(IEnumerable<UpdateVideoStreamRequest> VideoStreamUpdates) : IRequest
{
}

public class UpdateVideoStreamsRequestValidator : AbstractValidator<UpdateVideoStreamsRequest>
{
    public UpdateVideoStreamsRequestValidator()
    {
        _ = RuleFor(v => v.VideoStreamUpdates).NotNull().NotEmpty();
    }
}

public class UpdateVideoStreamsRequestHandler : BaseMemoryRequestHandler, IRequestHandler<UpdateVideoStreamsRequest>
{

    public UpdateVideoStreamsRequestHandler(ILogger<UpdateVideoStreamsRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }
    public async Task Handle(UpdateVideoStreamsRequest requests, CancellationToken cancellationToken)
    {
        bool refresh = false;
        bool refreshChannelGroup = false;

        foreach (UpdateVideoStreamRequest request in requests.VideoStreamUpdates)
        {
            bool ret = await Repository.VideoStream.UpdateVideoStreamAsync(request, cancellationToken).ConfigureAwait(false);
            if (ret)
            {
                refresh = true;
                if (request.IsHidden != null && !refreshChannelGroup)
                {
                    refreshChannelGroup = true;
                }
            }

        }

        if (refreshChannelGroup)
        {
            await Publisher.Publish(new UpdateChannelGroupEvent(), cancellationToken).ConfigureAwait(false);
        }

        if (refresh)
        {

            await Publisher.Publish(new UpdateVideoStreamsEvent(), cancellationToken).ConfigureAwait(false);
        }

    }
}
