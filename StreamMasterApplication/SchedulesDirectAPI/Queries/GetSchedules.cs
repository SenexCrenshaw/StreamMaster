using MediatR;

using StreamMaster.SchedulesDirectAPI;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetSchedules() : IRequest<List<Schedule>>;

internal class GetSchedulesHandler : IRequestHandler<GetSchedules, List<Schedule>>
{
    public async Task<List<Schedule>> Handle(GetSchedules request, CancellationToken cancellationToken)
    {
        var sd = new SchedulesDirect();
        var status = await sd.GetSystemReady(cancellationToken).ConfigureAwait(false);
        if (!status)
        {
            Console.WriteLine($"Status is {status}");
            return new();
        }
        var setting = FileUtil.GetSetting();
        if (setting.SDStationIds == null || !setting.SDStationIds.Any())
        {
            Console.WriteLine($"No station ids");
            return new();
        }

        var ret = await sd.GetSchedules(setting.SDStationIds, cancellationToken).ConfigureAwait(false);

        return ret ?? new();
    }
}
