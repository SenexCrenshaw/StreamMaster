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

[LogExecutionTimeAspect]

public class UpdateChannelGroupCountRequestByVideoStreamIdHandler(ILogger<UpdateChannelGroupCountRequestByVideoStreamId> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<UpdateChannelGroupCountRequestByVideoStreamId>
{
    public async Task Handle(UpdateChannelGroupCountRequestByVideoStreamId request, CancellationToken cancellationToken)
    {
        VideoStreamDto? videoStream = await Repository.VideoStream.GetVideoStreamById(request.videoStreamId).ConfigureAwait(false);
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
