using StreamMaster.SchedulesDirectAPI.Domain.Interfaces;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetEpg : IRequest<string>;

internal class GetEpgHandler(ISDService sdService) : IRequestHandler<GetEpg, string>
{
    [LogExecutionTimeAspect]
    public async Task<string> Handle(GetEpg request, CancellationToken cancellationToken)
    {
        //Setting setting = await settingsService.GetSettingsAsync(cancellationToken);

        //SchedulesDirect sd = new(setting.ClientUserAgent, setting.SDUserName, setting.SDPassword);
        //StreamMaster.SchedulesDirectAPI.Domain.Models.SDStatus status = await sd.GetStatus(cancellationToken).ConfigureAwait(false);
        //if (status?.systemStatus.Any() != true)
        //{
        //    Console.WriteLine("Status is null");
        //    return FileUtil.SerializeEpgData(new Tv());
        //}

        //StreamMaster.SchedulesDirectAPI.Domain.Models.SDSystemStatus systemStatus = status.systemStatus[0];
        //if (systemStatus.status == "Offline")
        //{
        //    Console.WriteLine($"Status is {systemStatus.status}");
        //    return FileUtil.SerializeEpgData(new Tv());
        //}

        string ret = await sdService.GetEpg(cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
