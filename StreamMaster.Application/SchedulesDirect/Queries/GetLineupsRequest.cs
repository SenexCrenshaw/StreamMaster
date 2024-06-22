namespace StreamMaster.Application.SchedulesDirect.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetLineupsRequest() : IRequest<DataResponse<List<SubscribedLineup>>>;

internal class GetLineupsRequestHandler(ILineups lineups) : IRequestHandler<GetLineupsRequest, DataResponse<List<SubscribedLineup>>>
{

    public async Task<DataResponse<List<SubscribedLineup>>> Handle(GetLineupsRequest request, CancellationToken cancellationToken)
    {
        var result = await lineups.GetLineups(cancellationToken);
        return DataResponse<List<SubscribedLineup>>.Success(result);
    }
}
