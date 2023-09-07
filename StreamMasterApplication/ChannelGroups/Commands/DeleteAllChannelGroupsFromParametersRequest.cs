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

public class DeleteAllChannelGroupsFromParametersRequestHandler(ILogger<DeleteAllChannelGroupsFromParametersRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMemoryRequestHandler(logger, repository, mapper, publisher, sender, hubContext, memoryCache), IRequestHandler<DeleteAllChannelGroupsFromParametersRequest, bool>
{
    public async Task<bool> Handle(DeleteAllChannelGroupsFromParametersRequest request, CancellationToken cancellationToken)
    {
        (IEnumerable<int> ChannelGroupIds, IEnumerable<VideoStreamDto> VideoStreams) = await Repository.ChannelGroup.DeleteAllChannelGroupsFromParameters(request.Parameters, cancellationToken).ConfigureAwait(false);

        if (ChannelGroupIds.Any())
        {
            foreach (int id in ChannelGroupIds)
            {
                MemoryCache.RemoveChannelGroupStreamCount(id);
            }

            await Publisher.Publish(new DeleteChannelGroupsEvent(ChannelGroupIds, VideoStreams), cancellationToken).ConfigureAwait(false);
            return true;
        }

        return false;

    }
}
