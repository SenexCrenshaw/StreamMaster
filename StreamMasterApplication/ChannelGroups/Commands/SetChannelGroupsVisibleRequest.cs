using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Hubs;
using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Attributes;

namespace StreamMasterApplication.ChannelGroups.Commands;

public record SetChannelGroupsVisibleArg(string GroupName, bool IsHidden)
{
}

public record SetVideoStreamVisibleRet(string videoStreamId, bool IsHidden)
{
}

[RequireAll]
public record SetChannelGroupsVisibleRequest(IEnumerable<SetChannelGroupsVisibleArg> requests) : IRequest<IEnumerable<SetChannelGroupsVisibleArg>>
{
}

public class SetChannelGroupsVisibleRequestValidator : AbstractValidator<SetChannelGroupsVisibleRequest>
{
}

public class SetChannelGroupsVisibleRequestHandler : BaseRequestHandler, IRequestHandler<SetChannelGroupsVisibleRequest, IEnumerable<SetChannelGroupsVisibleArg>>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

    public SetChannelGroupsVisibleRequestHandler(IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, ILogger<ChangeM3UFileNameRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper)
        : base(logger, repository, mapper)
    {
        _hubContext = hubContext;
    }

    public async Task<IEnumerable<SetChannelGroupsVisibleArg>> Handle(SetChannelGroupsVisibleRequest requests, CancellationToken cancellationToken)
    {
        bool isChanged = false;

        foreach (SetChannelGroupsVisibleArg request in requests.requests)
        {
            ChannelGroup? channelGroup = await Repository.ChannelGroup.GetChannelGroupByNameAsync(request.GroupName.ToLower()).ConfigureAwait(false);

            if (channelGroup == null)
            {
                continue;
            }

            if (channelGroup.IsHidden != request.IsHidden)
            {
                channelGroup.IsHidden = request.IsHidden;

                await Repository.VideoStream.SetGroupVisibleByGroupName(channelGroup.Name, request.IsHidden, cancellationToken).ConfigureAwait(false);
                await Repository.SaveAsync().ConfigureAwait(false);

                IQueryable<VideoStream> videoStreamsRepo = Repository.VideoStream.GetAllVideoStreams();
                List<VideoStream> videoStreams = videoStreamsRepo.ToList();

                IEnumerable<SetVideoStreamVisibleRet> changes = videoStreams
                    .Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroup.Name.ToLower())
                    .Select(a => new SetVideoStreamVisibleRet(a.Id, a.IsHidden));

                Repository.ChannelGroup.UpdateChannelGroup(channelGroup);

                isChanged = true;
            }
        }

        if (isChanged)
        {
            await Repository.SaveAsync().ConfigureAwait(false);

            await _hubContext.Clients.All.ChannelGroupsRefresh().ConfigureAwait(false);
            await _hubContext.Clients.All.VideoStreamsRefresh().ConfigureAwait(false);
        }
        return requests.requests;
    }
}