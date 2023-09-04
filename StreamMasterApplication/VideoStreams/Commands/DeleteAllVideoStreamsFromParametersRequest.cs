using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.ChannelGroups.Commands;
using StreamMasterApplication.ChannelGroups.Events;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.VideoStreams.Commands;

public record DeleteAllVideoStreamsFromParametersRequest(VideoStreamParameters Parameters) : IRequest { }

public class DeleteAllVideoStreamsFromParametersRequestValidator : AbstractValidator<DeleteAllVideoStreamsFromParametersRequest>
{
    public DeleteAllVideoStreamsFromParametersRequestValidator()
    {
        _ = RuleFor(v => v.Parameters).NotNull().NotEmpty();
    }
}

public class DeleteAllVideoStreamsFromParametersRequestHandler : BaseMemoryRequestHandler, IRequestHandler<DeleteAllVideoStreamsFromParametersRequest>
{

    public DeleteAllVideoStreamsFromParametersRequestHandler(ILogger<DeleteAllVideoStreamsFromParametersRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }
    public async Task Handle(DeleteAllVideoStreamsFromParametersRequest request, CancellationToken cancellationToken)
    {
        bool ret = await Repository.VideoStream.DeleteAllVideoStreamsFromParameters(request.Parameters, cancellationToken).ConfigureAwait(false);
        if (ret)
        {
            await Sender.Send(new UpdateChannelGroupCountsRequest(), cancellationToken).ConfigureAwait(false);
            await Publisher.Publish(new UpdateChannelGroupEvent(), cancellationToken).ConfigureAwait(false);
        }
    }
}
