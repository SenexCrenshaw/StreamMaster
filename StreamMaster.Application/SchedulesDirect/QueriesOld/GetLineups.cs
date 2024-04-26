namespace StreamMaster.Application.SchedulesDirect.QueriesOld;

public record GetLineups() : IRequest<List<SubscribedLineup>>;

internal class GetLineupsHandler(ILineups lineups) : IRequestHandler<GetLineups, List<SubscribedLineup>>
{

    public async Task<List<SubscribedLineup>> Handle(GetLineups request, CancellationToken cancellationToken)
    {
        var result = await lineups.GetLineups(cancellationToken);
        return result;
    }
}
