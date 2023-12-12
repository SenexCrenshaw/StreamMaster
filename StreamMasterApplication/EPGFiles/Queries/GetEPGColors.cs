using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMasterApplication.EPGFiles.Queries;

public record GetEPGColors() : IRequest<List<EPGColorDto>>;

internal class GetEPGColorsHandler(ILogger<GetEPGColors> logger, IRepositoryWrapper repository, ISchedulesDirectData schedulesDirectData, ISchedulesDirect schedulesDirect, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetEPGColors, List<EPGColorDto>>
{
    public async Task<List<EPGColorDto>> Handle(GetEPGColors request, CancellationToken cancellationToken = default)
    {
        List<EPGColorDto> ret = [];
        List<MxfService> svcs = schedulesDirectData.Services;

        List<EPGColorDto> epgColors = Repository.EPGFile.GetEPGColors();

        int index = 0;
        foreach (MxfService svc in svcs)
        {
            string color = "FFFFFF";
            int epgId = 0;

            if (svc.extras.TryGetValue("epgid", out dynamic? value))
            {
                epgId = value;
                EPGColorDto? epgColor = epgColors.FirstOrDefault(x => x.Id == epgId);
                color = epgColor?.Color ?? "FFFFFF";
            }

            ret.Add(new EPGColorDto
            {
                Id = index++,
                EPGFileId = epgId,
                CallSign = svc.CallSign,
                StationId = svc.StationId,
                Color = color,
            });

        }

        return ret;
    }
}
