using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.VideoStreams.Events;

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
        await Publisher.Publish(new UpdateVideoStreamEvent(), cancellationToken).ConfigureAwait(false);
    }
}