namespace StreamMaster.Application.SchedulesDirect.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetLineupPreviewChannelRequest(string Lineup) : IRequest<DataResponse<List<LineupPreviewChannel>>>;

internal class GetLineupPreviewChannelRequestHandler(ISchedulesDirectAPIService schedulesDirectAPIService)
    : IRequestHandler<GetLineupPreviewChannelRequest, DataResponse<List<LineupPreviewChannel>>>
{
    public async Task<DataResponse<List<LineupPreviewChannel>>> Handle(GetLineupPreviewChannelRequest request, CancellationToken cancellationToken)
    {
        List<LineupPreviewChannel>? result = await schedulesDirectAPIService.GetLineupPreviewChannelAsync(request.Lineup, cancellationToken);
        return result is null
            ? DataResponse<List<LineupPreviewChannel>>.Success([])
            : DataResponse<List<LineupPreviewChannel>>.Success(result);
    }
}
