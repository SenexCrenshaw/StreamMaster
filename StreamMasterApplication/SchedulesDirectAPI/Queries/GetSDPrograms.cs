using StreamMaster.SchedulesDirectAPI.Domain.Interfaces;
using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;
public record GetSDPrograms() : IRequest<List<SDProgram>>;

public class GetSDProgramsHandler(ISDService sdService, ILogger<GetSDPrograms> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetSDPrograms, List<SDProgram>>
{
    public async Task<List<SDProgram>> Handle(GetSDPrograms request, CancellationToken cancellationToken)
    {
        Setting setting = await GetSettingsAsync();
        //SchedulesDirect sd = new(setting.ClientUserAgent, setting.SDUserName, setting.SDPassword);
        List<Schedule> schedules = await Sender.Send(new GetSchedules(), cancellationToken).ConfigureAwait(false);

        if (schedules == null || !schedules.Any())
        {
            Console.WriteLine($"No schedules");
            return new();
        }

        List<string> progIds = schedules.SelectMany(a => a.Programs).Select(a => a.ProgramID).Distinct().ToList();

        List<SDProgram> ret = await sdService.GetSDPrograms(progIds, cancellationToken).ConfigureAwait(false);

        return ret ?? new();
    }
}
