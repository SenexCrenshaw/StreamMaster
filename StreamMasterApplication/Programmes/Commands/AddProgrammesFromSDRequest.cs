using StreamMaster.SchedulesDirectAPI;
using StreamMaster.SchedulesDirectAPI.Models;

namespace StreamMasterApplication.Programmes.Commands;

public record AddProgrammesFromSDRequest() : IRequest { }

public class AddProgrammesFromSDRequestHandler(ILogger<AddProgrammesFromSDRequestHandler> logger, ISettingsService settingsService) : IRequestHandler<AddProgrammesFromSDRequest>
{
    public async Task Handle(AddProgrammesFromSDRequest command, CancellationToken cancellationToken)
    {
        Setting setting = await settingsService.GetSettingsAsync();

        if (string.IsNullOrEmpty(setting.SDUserName) || string.IsNullOrEmpty(setting.SDPassword))
        {
            logger.LogDebug("No SD Information, skipping");
            return;
        }

        logger.LogInformation("Getting Token");

        SchedulesDirect sd = new(setting.ClientUserAgent, setting.SDPassword, setting.SDPassword);

        logger.LogInformation("Getting Status");
        SDStatus? status = await sd.GetStatus(cancellationToken);
        if (status == null || !status.systemStatus.Any())
        {
            logger.LogWarning("Status is null");
            return;
        }

        SDSystemstatus systemStatus = status.systemStatus[0];
        if (systemStatus.status == "Offline")
        {
            logger.LogWarning($"Status is {systemStatus.status}");
            return;
        }
        logger.LogInformation("System Is Online");

        LineUpResult? lineup = await sd.GetLineup("USA-OTA-19087", cancellationToken: cancellationToken);
        if (lineup == null)
        {
            logger.LogWarning($"lineup is null");
            return;
        }

        Station? wpvi = lineup.Stations.FirstOrDefault(a => a.Callsign.ToLower().Contains("wpvi"));
        if (wpvi != null)
        {
            logger.LogInformation("Getting Schedules");
            List<Schedule>? schedules = await sd.GetSchedules(new List<string> { wpvi.StationID }, cancellationToken: cancellationToken);
            if (schedules != null)
            {
                List<string> progIds = schedules.SelectMany(a => a.Programs).Select(a => a.ProgramID).Distinct().ToList();
                logger.LogInformation("Getting Programs");

                List<SDProgram>? programs = await sd.GetPrograms(progIds, cancellationToken: cancellationToken);
            }
        }
    }
}
