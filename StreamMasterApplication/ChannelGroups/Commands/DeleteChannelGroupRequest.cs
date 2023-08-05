using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Attributes;

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

public class DeleteChannelGroupRequestHandler : BaseMediatorRequestHandler, IRequestHandler<DeleteChannelGroupRequest, int?>
{
    public DeleteChannelGroupRequestHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }


    public async Task<int?> Handle(DeleteChannelGroupRequest request, CancellationToken cancellationToken)
    {
        ChannelGroup? channelGroup = await Repository.ChannelGroup.GetChannelGroupByNameAsync(request.GroupName.ToLower()).ConfigureAwait(false);

        if (channelGroup == null)
        {
            return null;
        }

        Repository.ChannelGroup.DeleteChannelGroup(channelGroup);
        await Repository.SaveAsync().ConfigureAwait(false);

        await Publisher.Publish(new DeleteChannelGroupEvent(channelGroup.Id), cancellationToken);

        return channelGroup.Id;
    }
}
