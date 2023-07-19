using MediatR;

using StreamMaster.SchedulesDirectAPI;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetStatus : IRequest<SDStatus>;

internal class GetStatusHandler : IRequestHandler<GetStatus, SDStatus>
{
    public async Task<SDStatus> Handle(GetStatus request, CancellationToken cancellationToken)
    {
        Setting setting = FileUtil.GetSetting();

        var sd = new SchedulesDirect(setting.SDUserName, setting.SDPassword);
        var status = await sd.GetStatus(cancellationToken).ConfigureAwait(false);
        return status ?? new SDStatus();
    }
}
