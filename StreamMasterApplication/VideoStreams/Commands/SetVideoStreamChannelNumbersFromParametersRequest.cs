using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.VideoStreams.Commands;

public record SetVideoStreamChannelNumbersFromParametersRequest(VideoStreamParameters Parameters, bool OverWriteExisting, int StartNumber) : IRequest { }

public class SetVideoStreamChannelNumbersFromParametersRequestHandler : BaseMediatorRequestHandler, IRequestHandler<SetVideoStreamChannelNumbersFromParametersRequest>
{

    public SetVideoStreamChannelNumbersFromParametersRequestHandler(ILogger<SetVideoStreamChannelNumbersFromParametersRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task Handle(SetVideoStreamChannelNumbersFromParametersRequest request, CancellationToken cancellationToken)
    {
        await Repository.VideoStream.SetVideoStreamChannelNumbersFromParameters(request.Parameters, request.OverWriteExisting, request.StartNumber, cancellationToken).ConfigureAwait(false);
        await Publisher.Publish(new UpdateVideoStreamEvent(), cancellationToken).ConfigureAwait(false);

    }
}