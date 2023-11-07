using StreamMaster.SchedulesDirectAPI.Domain.Interfaces;
using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetLineupNames : IRequest<List<string>>;

internal class GetLineupNamesHandler(ISDService sdService) : IRequestHandler<GetLineupNames, List<string>>
{
    public async Task<List<string>> Handle(GetLineupNames request, CancellationToken cancellationToken)
    {
        //Setting setting = await settingsService.GetSettingsAsync(cancellationToken);
        //SchedulesDirect sd = new(setting.ClientUserAgent, setting.SDUserName, setting.SDPassword);
        //bool isReady = await sd.GetSystemReady(cancellationToken).ConfigureAwait(false);
        //if (!isReady)
        //{
        //    Console.WriteLine($"Status is Offline");
        //    return new();
        //}

        List<StreamMaster.SchedulesDirectAPI.Domain.Models.Lineup> ret = await sdService.GetLineups(cancellationToken).ConfigureAwait(false);

        return ret.Select(a => a.Name).Distinct().Order().ToList();
    }
}