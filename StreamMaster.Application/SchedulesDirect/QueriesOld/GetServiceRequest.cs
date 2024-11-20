namespace StreamMaster.Application.SchedulesDirect.QueriesOld;

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
