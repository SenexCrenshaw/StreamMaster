using FluentValidation;

namespace StreamMasterApplication.StreamGroupVideoStreams.Commands;

[RequireAll]
public record AddVideoStreamToStreamGroupRequest(int StreamGroupId, string VideoStreamId) : IRequest { }

public class AddVideoStreamToStreamGroupRequestValidator : AbstractValidator<AddVideoStreamToStreamGroupRequest>
{
    public AddVideoStreamToStreamGroupRequestValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
           .NotNull()
           .GreaterThanOrEqualTo(0);
    }
}

public class AddVideoStreamToStreamGroupRequestHandler : BaseMediatorRequestHandler, IRequestHandler<AddVideoStreamToStreamGroupRequest>
{

    public AddVideoStreamToStreamGroupRequestHandler(ILogger<AddVideoStreamToStreamGroupRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
: base(logger, repository, mapper, publisher, sender, hubContext) { }

    public async Task Handle(AddVideoStreamToStreamGroupRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1)
        {
            return;
        }

        await Repository.StreamGroupVideoStream.AddVideoStreamToStreamGroup(request.StreamGroupId, request.VideoStreamId, cancellationToken).ConfigureAwait(false);

    }
}
