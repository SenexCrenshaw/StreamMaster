using FluentValidation;

namespace StreamMasterApplication.StreamGroupVideoStreams.Commands;

[RequireAll]
public record RemoveVideoStreamFromStreamGroupRequest(int StreamGroupId, string VideoStreamId) : IRequest { }

public class RemoveVideoStreamFromStreamGroupRequestValidator : AbstractValidator<RemoveVideoStreamFromStreamGroupRequest>
{
    public RemoveVideoStreamFromStreamGroupRequestValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
           .NotNull()
           .GreaterThanOrEqualTo(0);
    }
}

public class RemoveVideoStreamFromStreamGroupRequestHandler : BaseMediatorRequestHandler, IRequestHandler<RemoveVideoStreamFromStreamGroupRequest>
{

    public RemoveVideoStreamFromStreamGroupRequestHandler(ILogger<RemoveVideoStreamFromStreamGroupRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
: base(logger, repository, mapper, publisher, sender, hubContext) { }


    public async Task Handle(RemoveVideoStreamFromStreamGroupRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1)
        {
            return;
        }

        await Repository.StreamGroupVideoStream.RemoveVideoStreamFromStreamGroup(request.StreamGroupId, request.VideoStreamId, cancellationToken).ConfigureAwait(false);

    }
}
