using FluentValidation;

namespace StreamMaster.Application.StreamGroupVideoStreams.Commands;

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
public class SyncVideoStreamToStreamGroupRequestHandler(ILogger<SyncVideoStreamToStreamGroupRequest> logger, IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : IRequestHandler<SyncVideoStreamToStreamGroupRequest>
{
    public async Task Handle(SyncVideoStreamToStreamGroupRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1)
        {
            return;
        }

        StreamGroupDto? ret = await Repository.StreamGroupVideoStream.SyncVideoStreamToStreamGroup(request.StreamGroupId, request.VideoStreamId).ConfigureAwait(false);
        if (ret != null)
        {
            StreamGroupDto? dto = await Repository.StreamGroup.GetStreamGroupById(ret.Id).ConfigureAwait(false);
            await HubContext.Clients.All.StreamGroupsRefresh([dto]).ConfigureAwait(false);
        }
    }
}
