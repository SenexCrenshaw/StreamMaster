using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterDomain.Attributes;

namespace StreamMasterApplication.VideoStreams.Commands;

[RequireAll]
public record SetVideoStreamChannelNumbersRequest(List<string> Ids, bool OverWriteExisting, int StartNumber, string OrderBy) : IRequest { }

public class SetVideoStreamChannelNumbersRequestHandler : BaseMediatorRequestHandler, IRequestHandler<SetVideoStreamChannelNumbersRequest>
{

    public SetVideoStreamChannelNumbersRequestHandler(ILogger<SetVideoStreamChannelNumbersRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }


    public async Task Handle(SetVideoStreamChannelNumbersRequest request, CancellationToken cancellationToken)
    {
        string orderBy = string.IsNullOrEmpty(request.OrderBy) ? "user_tvg_name desc" : request.OrderBy;
        await Repository.VideoStream.SetVideoStreamChannelNumbersFromIds(request.Ids, request.OverWriteExisting, request.StartNumber, orderBy, cancellationToken).ConfigureAwait(false);
        await Publisher.Publish(new UpdateVideoStreamRequest(), cancellationToken).ConfigureAwait(false);

        //IQueryable<VideoStream> videoStreams = Repository.VideoStream.GetAllVideoStreams();

        //foreach (ChannelNumberPair cp in request.ChannelNumberPairs)
        //{
        //    VideoStream? videoStream = videoStreams.SingleOrDefault(c => c.Id == cp.Id);
        //    if (videoStream == null)
        //    {
        //        cp.Id = String.Empty;
        //        continue;
        //    }
        //    videoStream.User_Tvg_chno = cp.ChannelNumber;
        //    Repository.VideoStream.Update(videoStream);
        //}
        //await Repository.SaveAsync().ConfigureAwait(false);

        //await _hubContext.Clients.All.VideoStreamsRefresh().ConfigureAwait(false);

        //return request.ChannelNumberPairs;
    }
}