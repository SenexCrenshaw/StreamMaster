using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
    private readonly IAppDbContext _context;
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

    public SetChannelGroupsVisibleRequestHandler(IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, ILogger<ChangeM3UFileNameRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper)
        : base(logger, repository, mapper)
    {
        _hubContext = hubContext;
    }


    public async Task<IEnumerable<SetChannelGroupsVisibleArg>> Handle(SetChannelGroupsVisibleRequest requests, CancellationToken cancellationToken)
    {
        List<SetVideoStreamVisibleRet> ret = new();
        bool isChanged = false;

        foreach (var request in requests.requests)
        {
            ChannelGroup? channelGroup = await _context.ChannelGroups.FirstOrDefaultAsync(a => a.Name.ToLower() == request.GroupName.ToLower(), cancellationToken: cancellationToken).ConfigureAwait(false);

            if (channelGroup == null)
            {
                continue;
            }

            if (channelGroup.IsHidden != request.IsHidden)
            {
                channelGroup.IsHidden = request.IsHidden;

                await Repository.VideoStream.SetGroupVisibleByGroupName(channelGroup.Name, request.IsHidden, cancellationToken).ConfigureAwait(false);
                await Repository.SaveAsync().ConfigureAwait(false);

                var videoStreamsRepo = await Repository.VideoStream.GetAllVideoStreamsAsync().ConfigureAwait(false);
                var videoStreams = videoStreamsRepo.ToList();

                var changes = videoStreams
                    .Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroup.Name.ToLower())
                    .Select(a => new SetVideoStreamVisibleRet(a.Id, a.IsHidden));

                ret.AddRange(changes);

                isChanged = true;
            }
        }

        if (isChanged)
        {
            _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await _hubContext.Clients.All.ChannelGroupSetChannelGroupsVisible(requests.requests).ConfigureAwait(false);
            await _hubContext.Clients.All.VideoStreamSetVideoStreamVisible(ret).ConfigureAwait(false);
        }
        return requests.requests;
    }
}
