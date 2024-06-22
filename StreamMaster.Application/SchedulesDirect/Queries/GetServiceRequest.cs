namespace StreamMaster.Application.SchedulesDirect.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetServiceRequest(string stationId) : IRequest<DataResponse<MxfService?>>;

internal class GetServiceRequestHandler(ISchedulesDirectDataService schedulesDirectDataService)
    : IRequestHandler<GetServiceRequest, DataResponse<MxfService?>>
{

    public Task<DataResponse<MxfService?>> Handle(GetServiceRequest request, CancellationToken cancellationToken)
    {
        MxfService? service = schedulesDirectDataService.GetService(request.stationId);

        return Task.FromResult(DataResponse<MxfService?>.Success(service));
    }
}
