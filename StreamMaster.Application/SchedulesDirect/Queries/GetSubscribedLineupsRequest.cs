namespace StreamMaster.Application.SchedulesDirect.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetSubscribedLineupsRequest() : IRequest<DataResponse<List<SubscribedLineup>>>;

internal class GetSubscribedLineupsRequestHandler(ILineupService lineups) : IRequestHandler<GetSubscribedLineupsRequest, DataResponse<List<SubscribedLineup>>>
{
    public async Task<DataResponse<List<SubscribedLineup>>> Handle(GetSubscribedLineupsRequest request, CancellationToken cancellationToken)
    {
        var result = await lineups.GetLineups(cancellationToken);
        return DataResponse<List<SubscribedLineup>>.Success(result);
    }
}
