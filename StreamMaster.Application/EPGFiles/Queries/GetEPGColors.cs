namespace StreamMaster.Application.EPGFiles.Queries;

public record GetEPGColors() : IRequest<List<EPGColorDto>>;

internal class GetEPGColorsHandler(ILogger<GetEPGColors> logger, IRepositoryWrapper repository, ISchedulesDirectDataService schedulesDirectDataService, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetEPGColors, List<EPGColorDto>>
{
    public async Task<List<EPGColorDto>> Handle(GetEPGColors request, CancellationToken cancellationToken = default)
    {
        List<EPGColorDto> ret = [];
        List<MxfService> svcs = schedulesDirectDataService.AllServices;

        List<EPGColorDto> epgColors = Repository.EPGFile.GetEPGColors();

        int index = 0;
        foreach (MxfService svc in svcs)
        {
            string color = "#FFFFFF";

            if (svc.EPGNumber != 0)
            {
                EPGColorDto? epgColor = epgColors.FirstOrDefault(x => x.Id == svc.EPGNumber);
                color = epgColor?.Color ?? color;
            }

            ret.Add(new EPGColorDto
            {
                Id = index++,
                //EPGFileId = svc.EPGNumber,
                //CallSign = svc.CallSign,
                StationId = svc.StationId,
                Color = color,
            });

        }

        return ret;
    }
}
