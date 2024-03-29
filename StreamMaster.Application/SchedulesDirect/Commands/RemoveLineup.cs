﻿using StreamMaster.Domain.Configuration;

namespace StreamMaster.Application.SchedulesDirect.Commands;

public record RemoveLineup(string lineup) : IRequest<bool>;

public class RemoveLineupHandler(ISchedulesDirect schedulesDirect, IJobStatusService jobStatusService, ILogger<RemoveLineup> logger, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext, IOptionsMonitor<SDSettings> intsettings)
: IRequestHandler<RemoveLineup, bool>
{
    private readonly SDSettings sdsettings = intsettings.CurrentValue;

    public async Task<bool> Handle(RemoveLineup request, CancellationToken cancellationToken)
    {
        JobStatusManager jobManager = jobStatusService.GetJobManager(JobType.SDSync, EPGHelper.SchedulesDirectId);

        if (!sdsettings.SDEnabled)
        {
            return false;
        }
        logger.LogInformation("Remove line up {lineup}", request.lineup);
        if (await schedulesDirect.RemoveLineup(request.lineup, cancellationToken).ConfigureAwait(false))
        {
            if (await schedulesDirect.SDSync(cancellationToken))
            {
                await HubContext.Clients.All.SchedulesDirectsRefresh();

            }
            //schedulesDirect.ResetCache(SDCommands.Status);
            //schedulesDirect.ResetCache(SDCommands.LineUps);
            //await hubContext.Clients.All.SchedulesDirectsRefresh();
            schedulesDirect.ResetCache("SubscribedLineups");
            jobManager.SetForceNextRun();

            return true;
        }
        return false;
    }
}