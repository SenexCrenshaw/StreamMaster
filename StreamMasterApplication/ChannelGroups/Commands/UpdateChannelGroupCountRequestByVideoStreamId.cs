using FluentValidation;

using StreamMasterApplication.ChannelGroups.Queries;

namespace StreamMasterApplication.ChannelGroups.Commands;

public record UpdateChannelGroupCountRequestByVideoStreamId(string videoStreamId) : IRequest { }

public class UpdateChannelGroupCountRequestByVideoStreamIdValidator : AbstractValidator<UpdateChannelGroupCountRequestByVideoStreamId>
{
    public UpdateChannelGroupCountRequestByVideoStreamIdValidator()
    {
        _ = RuleFor(v => v.videoStreamId).NotNull().NotEmpty();
    }
}

public class UpdateChannelGroupCountRequestByVideoStreamIdHandler(ILogger<UpdateChannelGroupCountRequestByVideoStreamId> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMemoryRequestHandler(logger, repository, mapper, publisher, sender, hubContext, memoryCache), IRequestHandler<UpdateChannelGroupCountRequestByVideoStreamId>
{
    public async Task Handle(UpdateChannelGroupCountRequestByVideoStreamId request, CancellationToken cancellationToken)
    {
        VideoStream? videoStream = await Repository.VideoStream.GetVideoStreamByIdAsync(request.videoStreamId, cancellationToken).ConfigureAwait(false);
        if (videoStream == null)
        {
            return;
        }

        ChannelGroupDto? cg = await Sender.Send(new GetChannelGroupByName(videoStream.User_Tvg_group), cancellationToken).ConfigureAwait(false);
        if (cg == null)
        {
            return;
        }

        await Sender.Send(new UpdateChannelGroupCountRequest(cg), cancellationToken).ConfigureAwait(false);

    }
}
