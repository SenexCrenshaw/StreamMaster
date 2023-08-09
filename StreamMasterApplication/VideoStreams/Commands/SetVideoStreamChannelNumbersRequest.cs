using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Models;
using StreamMasterApplication.Hubs;
using StreamMasterApplication.M3UFiles.Commands;

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

public class SetVideoStreamChannelNumbersRequestHandler : BaseRequestHandler, IRequestHandler<SetVideoStreamChannelNumbersRequest, IEnumerable<ChannelNumberPair>>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

    public SetVideoStreamChannelNumbersRequestHandler(IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, ILogger<ChangeM3UFileNameRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper)
        : base(logger, repository, mapper)
    {
        _hubContext = hubContext;
    }

    public async Task<IEnumerable<ChannelNumberPair>> Handle(SetVideoStreamChannelNumbersRequest request, CancellationToken cancellationToken)
    {
        IQueryable<VideoStream> videoStreams = Repository.VideoStream.GetAllVideoStreams();

        foreach (ChannelNumberPair cp in request.ChannelNumberPairs)
        {
            VideoStream? videoStream = videoStreams.SingleOrDefault(c => c.Id == cp.Id);
            if (videoStream == null)
            {
                cp.Id = String.Empty;
                continue;
            }
            videoStream.User_Tvg_chno = cp.ChannelNumber;
            Repository.VideoStream.Update(videoStream);
        }
        await Repository.SaveAsync().ConfigureAwait(false);

        await _hubContext.Clients.All.VideoStreamsRefresh().ConfigureAwait(false);

        return request.ChannelNumberPairs;
    }
}