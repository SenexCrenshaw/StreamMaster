using FluentValidation;

using StreamMaster.Application.ChannelGroups.Commands;
using StreamMaster.Application.VideoStreams.Events;
using StreamMaster.Domain.Requests;

namespace StreamMaster.Application.VideoStreams.Commands;

public class UpdateVideoStreamsRequestValidator : AbstractValidator<UpdateVideoStreamsRequest>
{
    public UpdateVideoStreamsRequestValidator()
    {
        _ = RuleFor(v => v.VideoStreamUpdates).NotNull().NotEmpty();
    }
}

[LogExecutionTimeAspect]
public class UpdateVideoStreamsRequestHandler(ILogger<UpdateVideoStreamsRequest> logger, IRepositoryWrapper Repository, IPublisher Publisher, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : IRequestHandler<UpdateVideoStreamsRequest, List<VideoStreamDto>>
{
    public async Task<List<VideoStreamDto>> Handle(UpdateVideoStreamsRequest requests, CancellationToken cancellationToken)
    {
        (List<VideoStreamDto> videoStreams, List<ChannelGroupDto>? updatedChannelGroups) = await Repository.VideoStream.UpdateVideoStreamsAsync(requests.VideoStreamUpdates, cancellationToken);
        if (videoStreams.Any())
        {
            bool toggleVisibilty = requests.VideoStreamUpdates.Any(x => x.ToggleVisibility.HasValue);
            if (toggleVisibilty)
            {
                await Publisher.Publish(new UpdateVideoStreamsEvent([]), cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await Publisher.Publish(new UpdateVideoStreamsEvent(videoStreams), cancellationToken).ConfigureAwait(false);
            }

            if (updatedChannelGroups.Any())
            {
                updatedChannelGroups = (await Sender.Send(new UpdateChannelGroupCountsRequest(updatedChannelGroups), cancellationToken).ConfigureAwait(false)).Data;
                await HubContext.Clients.All.ChannelGroupsRefresh(updatedChannelGroups.ToArray()).ConfigureAwait(false);
            }
        }

        return videoStreams;
    }
}