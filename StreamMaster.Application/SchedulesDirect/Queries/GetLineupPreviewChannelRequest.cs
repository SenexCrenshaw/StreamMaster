namespace StreamMaster.Application.SchedulesDirect.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetLineupPreviewChannelRequest(string Lineup) : IRequest<DataResponse<List<LineupPreviewChannel>>>;

internal class GetLineupPreviewChannelRequestHandler(ISchedulesDirect schedulesDirect)
    : IRequestHandler<GetLineupPreviewChannelRequest, DataResponse<List<LineupPreviewChannel>>>
{

    public async Task<DataResponse<List<LineupPreviewChannel>>> Handle(GetLineupPreviewChannelRequest request, CancellationToken cancellationToken)
    {
        List<LineupPreviewChannel>? result = await schedulesDirect.GetLineupPreviewChannel(request.Lineup, cancellationToken);

        return DataResponse<List<LineupPreviewChannel>>.Success(result);
    }
}
