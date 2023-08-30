using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Attributes;

namespace StreamMasterApplication.StreamGroups.Commands;

[RequireAll]
public record RemoveVideoStreamFromVideoStreamRequest(string ParentVideoStreamId, string ChildVideoStreamId) : IRequest { }

public class RemoveVideoStreamFromVideoStreamRequestValidator : AbstractValidator<RemoveVideoStreamFromVideoStreamRequest>
{
    public RemoveVideoStreamFromVideoStreamRequestValidator()
    {
    }
}

public class RemoveVideoStreamFromVideoStreamRequestHandler : BaseMediatorRequestHandler, IRequestHandler<RemoveVideoStreamFromVideoStreamRequest>
{
    public RemoveVideoStreamFromVideoStreamRequestHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task Handle(RemoveVideoStreamFromVideoStreamRequest request, CancellationToken cancellationToken)
    {
        await Repository.VideoStream.RemoveVideoStreamFromVideoStream(request.ParentVideoStreamId, request.ChildVideoStreamId, cancellationToken);
        await Publisher.Publish(new StreamGroupUpdateEvent(), cancellationToken).ConfigureAwait(false);
    }
}