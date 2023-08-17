using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.ChannelGroups.Commands;
using StreamMasterApplication.ChannelGroups.Queries;
using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Commands;

public class UpdateVideoStreamRequest : VideoStreamUpdate, IRequest<VideoStreamDto?>
{
}

public class UpdateVideoStreamRequestValidator : AbstractValidator<UpdateVideoStreamRequest>
{
    public UpdateVideoStreamRequestValidator()
    {
        _ = RuleFor(v => v.Id).NotNull().NotEmpty();
    }
}

public class UpdateVideoStreamRequestHandler : BaseMemoryRequestHandler, IRequestHandler<UpdateVideoStreamRequest, VideoStreamDto?>
{

    public UpdateVideoStreamRequestHandler(ILogger<UpdateVideoStreamRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache)
    { }

    public async Task<VideoStreamDto?> Handle(UpdateVideoStreamRequest request, CancellationToken cancellationToken)
    {
        VideoStreamDto? ret = await Repository.VideoStream.UpdateVideoStreamAsync(request, cancellationToken).ConfigureAwait(false);

        if (ret is not null)
        {
            if (request.IsHidden != null)
            {
                List<string> channelnames = await Sender.Send(new GetChannelGroupNamesFromVideoStream(ret), cancellationToken).ConfigureAwait(false);
                await Sender.Send(new UpdateChannelGroupCountsRequest(channelnames), cancellationToken).ConfigureAwait(false);
            }
            await Publisher.Publish(new UpdateVideoStreamEvent(), cancellationToken).ConfigureAwait(false);
        }

        return ret;
    }
}
