using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.ChannelGroups.Events;
using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Attributes;

namespace StreamMasterApplication.ChannelGroups.Commands;

[RequireAll]
public record CreateChannelGroupRequest(string GroupName, int Rank) : IRequest { }

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
        Repository.ChannelGroup.CreateChannelGroup(channelGroup);
        await Repository.SaveAsync().ConfigureAwait(false);

        await Publisher.Publish(new AddChannelGroupEvent(), cancellationToken).ConfigureAwait(false);


    }
}
