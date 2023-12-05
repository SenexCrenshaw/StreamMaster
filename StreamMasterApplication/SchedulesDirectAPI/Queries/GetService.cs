using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetService(string stationId) : IRequest<MxfService?>;

internal class GetServiceHandler(ISchedulesDirectData schedulesDirectData) : IRequestHandler<GetService, MxfService?>
{

    public  Task<MxfService?> Handle(GetService request, CancellationToken cancellationToken)
    {
        var service = schedulesDirectData.GetService(request.stationId);

        return Task.FromResult(service);
    }
}
