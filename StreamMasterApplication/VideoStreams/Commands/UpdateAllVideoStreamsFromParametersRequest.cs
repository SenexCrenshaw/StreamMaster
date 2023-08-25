using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.ChannelGroups.Events;
using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.VideoStreams.Commands;

public record UpdateAllVideoStreamsFromParametersRequest(VideoStreamParameters Parameters, UpdateVideoStreamRequest request) : IRequest
{
}

public class UpdateAllVideoStreamsFromParametersRequestValidator : AbstractValidator<UpdateAllVideoStreamsFromParametersRequest>
{
    public UpdateAllVideoStreamsFromParametersRequestValidator()
    {
        _ = RuleFor(v => v.Parameters).NotNull().NotEmpty();
    }
}

public class UpdateAllVideoStreamsFromParametersRequestHandler : BaseMemoryRequestHandler, IRequestHandler<UpdateAllVideoStreamsFromParametersRequest>
{

    public UpdateAllVideoStreamsFromParametersRequestHandler(ILogger<UpdateAllVideoStreamsFromParametersRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }
    public async Task Handle(UpdateAllVideoStreamsFromParametersRequest request, CancellationToken cancellationToken)
    {
        bool refreshChannelGroup = false;

        bool ret = await Repository.VideoStream.UpdateAllVideoStreamsFromParameters(request.Parameters, request.request, cancellationToken).ConfigureAwait(false);
        if (ret)
        {
            if (request.request.IsHidden != null && !refreshChannelGroup)
            {
                refreshChannelGroup = true;
            }
        }


        if (refreshChannelGroup)
        {
            await Publisher.Publish(new UpdateChannelGroupEvent(), cancellationToken).ConfigureAwait(false);
        }

        await Publisher.Publish(new UpdateVideoStreamsEvent(), cancellationToken).ConfigureAwait(false);

    }
}
