using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMasterApplication.Common.Models;
using StreamMasterApplication.Hubs;

using StreamMasterDomain.Attributes;

namespace StreamMasterApplication.VideoStreams.Commands;

[RequireAll]
public class SetVideoStreamChannelNumbersRequest : IRequest<IEnumerable<ChannelNumberPair>>
{
    public SetVideoStreamChannelNumbersRequest()
    {
        ChannelNumberPairs = new List<ChannelNumberPair>();
    }

    public List<ChannelNumberPair> ChannelNumberPairs { get; set; }
}

public class SetVideoStreamChannelNumbersRequestHandler : IRequestHandler<SetVideoStreamChannelNumbersRequest, IEnumerable<ChannelNumberPair>>
{
    private readonly IAppDbContext _context;
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

    public SetVideoStreamChannelNumbersRequestHandler(
        IHubContext<StreamMasterHub, IStreamMasterHub> hubContext,
        IAppDbContext Context)
    {
        _hubContext = hubContext;
        _context = Context;
    }

    public async Task<IEnumerable<ChannelNumberPair>> Handle(SetVideoStreamChannelNumbersRequest request, CancellationToken cancellationToken)
    {
        foreach (ChannelNumberPair cp in request.ChannelNumberPairs)
        {
            var VideoStream = _context.VideoStreams.SingleOrDefault(c => c.Id == cp.Id);
            if (VideoStream == null)
            {
                cp.Id = String.Empty;
                continue;
            }
            VideoStream.User_Tvg_chno = cp.ChannelNumber;
        }
        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        await _hubContext.Clients.All.VideoStreamUpdateChannelNumbers(request.ChannelNumberPairs).ConfigureAwait(false);

        return request.ChannelNumberPairs;
    }
}
