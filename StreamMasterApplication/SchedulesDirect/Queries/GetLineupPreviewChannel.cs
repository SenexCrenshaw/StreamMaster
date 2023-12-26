namespace StreamMasterApplication.SchedulesDirect.Queries;

public record GetLineupPreviewChannel(string Lineup) : IRequest<List<LineupPreviewChannel>>;

internal class GetLineupPreviewChannelHandler(ISchedulesDirect schedulesDirect, IMapper mapper) : IRequestHandler<GetLineupPreviewChannel, List<LineupPreviewChannel>>
{

    public async Task<List<LineupPreviewChannel>> Handle(GetLineupPreviewChannel request, CancellationToken cancellationToken)
    {
        var result = await schedulesDirect.GetLineupPreviewChannel(request.Lineup, cancellationToken);
        return result ?? [];
    }
}
