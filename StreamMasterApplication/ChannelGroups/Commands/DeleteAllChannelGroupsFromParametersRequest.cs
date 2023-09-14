using FluentValidation;

using StreamMasterApplication.ChannelGroups.Events;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.ChannelGroups.Commands;

public record DeleteAllChannelGroupsFromParametersRequest(ChannelGroupParameters Parameters) : IRequest<bool>
{
}

public class DeleteAllChannelGroupsFromParametersRequestValidator : AbstractValidator<DeleteAllChannelGroupsFromParametersRequest>
{
    public DeleteAllChannelGroupsFromParametersRequestValidator()
    {
        _ = RuleFor(v => v.Parameters).NotNull().NotEmpty();
    }
}

public class DeleteAllChannelGroupsFromParametersRequestHandler(ILogger<DeleteAllChannelGroupsFromParametersRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<DeleteAllChannelGroupsFromParametersRequest, bool>
{
    public async Task<bool> Handle(DeleteAllChannelGroupsFromParametersRequest request, CancellationToken cancellationToken)
    {
        (IEnumerable<int> ChannelGroupIds, IEnumerable<VideoStreamDto> VideoStreams) = await Repository.ChannelGroup.DeleteAllChannelGroupsFromParameters(request.Parameters, cancellationToken).ConfigureAwait(false);
        await Repository.VideoStream.UpdateVideoStreamsChannelGroupNames(VideoStreams.Select(a => a.Id), "").ConfigureAwait(false);
        _ = await Repository.SaveAsync().ConfigureAwait(false);

        if (ChannelGroupIds.Any())
        {

            foreach (int id in ChannelGroupIds)
            {
                MemoryCache.RemoveChannelGroupStreamCount(id);
            }


            foreach (VideoStreamDto item in VideoStreams)
            {
                item.User_Tvg_group = "";
            }
            await Publisher.Publish(new DeleteChannelGroupsEvent(ChannelGroupIds, VideoStreams), cancellationToken);

            return true;
        }

        return false;
    }
}
