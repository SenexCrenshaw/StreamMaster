using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.ChannelGroups.Events;
using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Attributes;

namespace StreamMasterApplication.ChannelGroups.Commands;

[RequireAll]
#if HAS_REGEX
public record CreateChannelGroupRequest(string GroupName, int Rank, string? Regex) : IRequest{}
#else
public record CreateChannelGroupRequest(string GroupName, int Rank) : IRequest { }
#endif

public class CreateChannelGroupRequestValidator : AbstractValidator<CreateChannelGroupRequest>
{
    public CreateChannelGroupRequestValidator()
    {
        _ = RuleFor(v => v.GroupName).NotNull().NotEmpty();
        _ = RuleFor(v => v.Rank).NotNull().GreaterThan(0);
    }
}

public class CreateChannelGroupRequestHandler : BaseMediatorRequestHandler, IRequestHandler<CreateChannelGroupRequest>
{

    public CreateChannelGroupRequestHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task Handle(CreateChannelGroupRequest request, CancellationToken cancellationToken)
    {
        if (await Repository.ChannelGroup.GetChannelGroupByNameAsync(request.GroupName).ConfigureAwait(false) != null)
        {
            return;
        }

        ChannelGroup channelGroup = new() { Name = request.GroupName, Rank = request.Rank, IsReadOnly = false };

#if HAS_REGEX
        if (!string.IsNullOrEmpty(request.Regex))
        {
            channelGroup.RegexMatch = request.Regex;
        }
#endif
        Repository.ChannelGroup.CreateChannelGroup(channelGroup);
        await Repository.SaveAsync().ConfigureAwait(false);
#if HAS_REGEX
        if (string.IsNullOrEmpty(request.Regex))
        {
            await Repository.ChannelGroup.ChannelGroupCreateEmptyCount(channelGroup.Id).ConfigureAwait(false);
        }
        else
        {
            await Sender.Send(new UpdateChannelGroupCountRequest(channelGroup.Name), cancellationToken).ConfigureAwait(false);
        }

#endif

        await Publisher.Publish(new AddChannelGroupEvent(), cancellationToken).ConfigureAwait(false);

#if HAS_REGEX
        if (!string.IsNullOrEmpty(request.Regex))
        {
            await Publisher.Publish(new UpdateVideoStreamEvent(), cancellationToken).ConfigureAwait(false);
        }
#endif
    }
}
