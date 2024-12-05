namespace StreamMaster.Application.SchedulesDirect.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetSubscribedLineupsRequest() : IRequest<DataResponse<List<SubscribedLineup>>>;

internal class GetSubscribedLineupsRequestHandler(ISchedulesDirectAPIService schedulesDirectAPI) : IRequestHandler<GetSubscribedLineupsRequest, DataResponse<List<SubscribedLineup>>>
{
    public async Task<DataResponse<List<SubscribedLineup>>> Handle(GetSubscribedLineupsRequest request, CancellationToken cancellationToken)
    {
        LineupResponse? lineups = await schedulesDirectAPI.GetSubscribedLineupsAsync(cancellationToken).ConfigureAwait(false);

        return DataResponse<List<SubscribedLineup>>.Success(lineups?.Lineups ?? []);
    }
}
