namespace StreamMaster.Application.EPGFiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetEPGColorsRequest() : IRequest<List<EPGColorDto>>;

internal class GetEPGColorsHandler(IRepositoryWrapper Repository, ISchedulesDirectDataService schedulesDirectDataService)
    : IRequestHandler<GetEPGColorsRequest, List<EPGColorDto>>
{
    public async Task<List<EPGColorDto>> Handle(GetEPGColorsRequest request, CancellationToken cancellationToken = default)
    {
        List<EPGColorDto> ret = [];
        List<MxfService> svcs = schedulesDirectDataService.AllServices;

        List<EPGColorDto> epgColors = Repository.EPGFile.GetEPGColors();

        int index = 0;
        foreach (MxfService svc in svcs)
        {
            string color = "#FFFFFF";

            if (svc.EPGNumber != EPGHelper.SchedulesDirectId)
            {
                EPGColorDto? epgColor = epgColors.FirstOrDefault(x => x.EPGNumber == svc.EPGNumber);
                color = epgColor?.Color ?? color;
            }

            ret.Add(new EPGColorDto
            {
                Id = index++,
                StationId = svc.StationId,
                Color = color,
            });

        }

        return ret;
    }
}
