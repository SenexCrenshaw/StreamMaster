using FluentValidation;

using StreamMaster.Domain.Models;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

namespace StreamMaster.Application.StreamGroupVideoStreams.Commands;

[RequireAll]
public record SetVideoStreamRanksRequest(int StreamGroupId, List<VideoStreamIDRank> VideoStreamIDRanks) : IRequest { }

public class SetVideoStreamRanksRequestValidator : AbstractValidator<SetVideoStreamRanksRequest>
{
    public SetVideoStreamRanksRequestValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
           .NotNull()
           .GreaterThanOrEqualTo(0);
    }
}

[LogExecutionTimeAspect]
public class SetVideoStreamRanksRequestHandler(ILogger<SetVideoStreamRanksRequest> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper,settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<SetVideoStreamRanksRequest>
{
    public async Task Handle(SetVideoStreamRanksRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1)
        {
            return;
        }

        await Repository.StreamGroupVideoStream.SetVideoStreamRanks(request.StreamGroupId, request.VideoStreamIDRanks, cancellationToken).ConfigureAwait(false);

    }
}
