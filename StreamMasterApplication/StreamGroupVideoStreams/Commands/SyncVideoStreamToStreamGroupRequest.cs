using FluentValidation;

namespace StreamMasterApplication.StreamGroupVideoStreams.Commands;

[RequireAll]
public record SyncVideoStreamToStreamGroupRequest(int StreamGroupId, string VideoStreamId) : IRequest { }

public class SyncVideoStreamToStreamGroupRequestValidator : AbstractValidator<SyncVideoStreamToStreamGroupRequest>
{
    public SyncVideoStreamToStreamGroupRequestValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
           .NotNull()
           .GreaterThanOrEqualTo(0);
    }
}

[LogExecutionTimeAspect]
public class SyncVideoStreamToStreamGroupRequestHandler(ILogger<SyncVideoStreamToStreamGroupRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<SyncVideoStreamToStreamGroupRequest>
{
    public async Task Handle(SyncVideoStreamToStreamGroupRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1)
        {
            return;
        }

        StreamGroupDto? ret = await Repository.StreamGroupVideoStream.SyncVideoStreamToStreamGroup(request.StreamGroupId, request.VideoStreamId, cancellationToken).ConfigureAwait(false);
        if (ret != null)
        {
            StreamGroupDto? dto = await repository.StreamGroup.GetStreamGroupById(ret.Id).ConfigureAwait(false);
            await HubContext.Clients.All.StreamGroupsRefresh([dto]).ConfigureAwait(false);
        }
    }
}
