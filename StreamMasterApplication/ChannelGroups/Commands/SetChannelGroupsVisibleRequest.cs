using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.Hubs;

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

public class SetChannelGroupsVisibleRequestHandler : IRequestHandler<SetChannelGroupsVisibleRequest, IEnumerable<SetChannelGroupsVisibleArg>>
{
    private readonly IAppDbContext _context;
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

    public SetChannelGroupsVisibleRequestHandler(
        IHubContext<StreamMasterHub, IStreamMasterHub> hubContext,
        IAppDbContext context
        )
    {
        _hubContext = hubContext;
        _context = context;
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

                await _context.VideoStreams
                    .Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroup.Name.ToLower())
                    .ExecuteUpdateAsync(s => s.SetProperty(b => b.IsHidden, request.IsHidden), cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                var changes = _context.VideoStreams
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
