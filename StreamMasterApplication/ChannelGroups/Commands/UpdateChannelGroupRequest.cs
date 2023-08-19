using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.ChannelGroups.Events;
using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Dto;

using System.Diagnostics;

namespace StreamMasterApplication.ChannelGroups.Commands;

public class UpdateChannelGroupRequestValidator : AbstractValidator<UpdateChannelGroupRequest>
{
    public UpdateChannelGroupRequestValidator()
    {
        _ = RuleFor(v => v.ChannelGroupName).NotNull().NotEmpty();
        _ = RuleFor(v => v.Rank).NotNull().GreaterThan(0);
    }
}

public class UpdateChannelGroupRequestHandler : BaseMediatorRequestHandler, IRequestHandler<UpdateChannelGroupRequest>
{

    public UpdateChannelGroupRequestHandler(ILogger<UpdateChannelGroupRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender)
    {

    }

    public async Task Handle(UpdateChannelGroupRequest request, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        ChannelGroup? channelGroup = await Repository.ChannelGroup.GetChannelGroupByNameAsync(request.ChannelGroupName).ConfigureAwait(false);
        bool checkCounts = false;
        if (channelGroup == null)
        {
            return;
        }

        if (request.Rank != null && (int)request.Rank != request.Rank)
        {
            channelGroup.Rank = (int)request.Rank;
        }

        if (request.IsHidden != null && channelGroup.IsHidden != (bool)request.IsHidden)
        {
            channelGroup.IsHidden = (bool)request.IsHidden;

            int results = await Repository.VideoStream.SetGroupVisibleByGroupName(channelGroup.Name, (bool)request.IsHidden, cancellationToken).ConfigureAwait(false);
            checkCounts = results > 0;

        }

        if (!string.IsNullOrEmpty(request.NewGroupName) && request.NewGroupName != channelGroup.Name)
        {
            channelGroup.Name = request.NewGroupName;
            await Repository.VideoStream.SetGroupNameByGroupName(channelGroup.Name, request.NewGroupName, cancellationToken).ConfigureAwait(false);
        }

        Repository.ChannelGroup.UpdateChannelGroup(channelGroup);
        await Repository.SaveAsync().ConfigureAwait(false);

        await Publisher.Publish(new UpdateChannelGroupEvent(), cancellationToken).ConfigureAwait(false);

        if (checkCounts)
        {
            await Sender.Send(new UpdateChannelGroupCountRequest(channelGroup.Name), cancellationToken).ConfigureAwait(false);
            await Publisher.Publish(new UpdateVideoStreamEvent(), cancellationToken).ConfigureAwait(false);
        }

        stopwatch.Stop();
        Logger.LogInformation($"UpdateChannelGroupRequestHandler took {stopwatch.ElapsedMilliseconds} ms");

    }
}
