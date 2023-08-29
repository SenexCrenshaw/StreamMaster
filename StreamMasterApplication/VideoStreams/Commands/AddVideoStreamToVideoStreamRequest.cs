using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Attributes;

namespace StreamMasterApplication.StreamGroups.Commands;

[RequireAll]
public record AddVideoStreamToVideoStreamRequest(string ParentVideoStreamId, CreateVideoStreamRequest createVideoStreamRequest) : IRequest { }

public class AddVideoStreamToVideoStreamRequestValidator : AbstractValidator<AddVideoStreamToVideoStreamRequest>
{
    public AddVideoStreamToVideoStreamRequestValidator()
    {
      
    }
}

public class AddVideoStreamToVideoStreamRequestHandler : BaseMediatorRequestHandler, IRequestHandler<AddVideoStreamToVideoStreamRequest >
{
    public AddVideoStreamToVideoStreamRequestHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender)    {    }

    public async Task Handle(AddVideoStreamToVideoStreamRequest request, CancellationToken cancellationToken)
    {

        await Repository.VideoStream.AddVideoStreamTodVideoStream(request.ParentVideoStreamId, request.createVideoStreamRequest).ConfigureAwait(false);
      
      
        await Publisher.Publish(new StreamGroupUpdateEvent(), cancellationToken).ConfigureAwait(false);
        
    }
}
