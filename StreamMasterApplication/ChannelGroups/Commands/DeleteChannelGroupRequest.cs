using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.ChannelGroups.Events;
using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Cache;

namespace StreamMasterApplication.ChannelGroups.Commands;

[RequireAll]
public record DeleteChannelGroupRequest(string GroupName) : IRequest<int?>
{
}

public class DeleteChannelGroupRequestValidator : AbstractValidator<DeleteChannelGroupRequest>
{
    public DeleteChannelGroupRequestValidator()
    {
        _ = RuleFor(v => v.GroupName).NotNull().NotEmpty();
    }
}

public class DeleteChannelGroupRequestHandler : BaseMemoryRequestHandler, IRequestHandler<DeleteChannelGroupRequest, int?>
{
    public DeleteChannelGroupRequestHandler(ILogger<DeleteChannelGroupRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }


    public async Task<int?> Handle(DeleteChannelGroupRequest request, CancellationToken cancellationToken)
    {
        ChannelGroup? channelGroup = await Repository.ChannelGroup.GetChannelGroupByNameAsync(request.GroupName).ConfigureAwait(false);

        if (channelGroup == null)
        {
            return null;
        }

        Repository.ChannelGroup.DeleteChannelGroup(channelGroup);
        await Repository.SaveAsync().ConfigureAwait(false);
        MemoryCache.RemoveChannelGroupStreamCount(channelGroup.Id);
        await Publisher.Publish(new DeleteChannelGroupEvent(channelGroup.Id), cancellationToken);
        await Publisher.Publish(new UpdateVideoStreamEvent(), cancellationToken);

        return channelGroup.Id;
    }
}
