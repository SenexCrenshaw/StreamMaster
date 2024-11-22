namespace StreamMaster.Application.SchedulesDirect.QueriesOld;

public record GetServiceRequest(string StationId) : IRequest<DataResponse<MxfService?>>;

internal class GetServiceRequestHandler(ISchedulesDirectDataService schedulesDirectDataService)
    : IRequestHandler<GetServiceRequest, DataResponse<MxfService?>>
{
    public Task<DataResponse<MxfService?>> Handle(GetServiceRequest request, CancellationToken cancellationToken)
    {
        MxfService? service = schedulesDirectDataService.GetService(request.StationId);

        return Task.FromResult(DataResponse<MxfService?>.Success(service));
    }
}
