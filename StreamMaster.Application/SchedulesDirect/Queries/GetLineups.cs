namespace StreamMaster.Application.SchedulesDirect.Queries;

public record GetLineups() : IRequest<List<SubscribedLineup>>;

internal class GetLineupsHandler(ISchedulesDirect schedulesDirect, IMapper mapper) : IRequestHandler<GetLineups, List<SubscribedLineup>>
{

    public async Task<List<SubscribedLineup>> Handle(GetLineups request, CancellationToken cancellationToken)
    {
        var result = await schedulesDirect.GetLineups(cancellationToken);
        return result;
    }
}
