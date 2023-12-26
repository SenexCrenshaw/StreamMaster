namespace StreamMaster.Application.SchedulesDirect.Queries;

public record GetService(string stationId) : IRequest<MxfService?>;

internal class GetServiceHandler(ISchedulesDirectDataService schedulesDirectDataService) : IRequestHandler<GetService, MxfService?>
{

    public Task<MxfService?> Handle(GetService request, CancellationToken cancellationToken)
    {
        MxfService? service = schedulesDirectDataService.GetService(request.stationId);

        return Task.FromResult(service);
    }
}
