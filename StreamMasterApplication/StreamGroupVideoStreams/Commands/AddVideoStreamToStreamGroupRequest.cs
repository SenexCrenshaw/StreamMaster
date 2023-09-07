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

public class AddVideoStreamToStreamGroupRequestHandler(ILogger<AddVideoStreamToStreamGroupRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper, publisher, sender, hubContext), IRequestHandler<AddVideoStreamToStreamGroupRequest>
{
    public async Task Handle(AddVideoStreamToStreamGroupRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1)
        {
            return;
        }

        StreamGroupDto? ret = await Repository.StreamGroupVideoStream.AddVideoStreamToStreamGroup(request.StreamGroupId, request.VideoStreamId, cancellationToken).ConfigureAwait(false);
        if (ret != null)
        {
            await HubContext.Clients.All.StreamGroupsRefresh([ret]).ConfigureAwait(false);
        }
    }
}
