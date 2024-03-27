using FluentValidation;

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
public class SetVideoStreamRanksRequestHandler(ILogger<SetVideoStreamRanksRequest> logger, IRepositoryWrapper Repository)
    : IRequestHandler<SetVideoStreamRanksRequest>
{
    public async Task Handle(SetVideoStreamRanksRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1)
        {
            return;
        }

        await Repository.StreamGroupVideoStream.SetVideoStreamRanks(request.StreamGroupId, request.VideoStreamIDRanks).ConfigureAwait(false);

    }
}
