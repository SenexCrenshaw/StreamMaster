namespace StreamMaster.Application.EPG.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetEPGColorsRequest() : IRequest<DataResponse<List<EPGColorDto>>>;

internal class GetEPGColorsHandler(IRepositoryWrapper Repository, ISchedulesDirectDataService schedulesDirectDataService)
    : IRequestHandler<GetEPGColorsRequest, DataResponse<List<EPGColorDto>>>
{
    public async Task<DataResponse<List<EPGColorDto>>> Handle(GetEPGColorsRequest request, CancellationToken cancellationToken = default)
    {

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

            epgColors.Add(new EPGColorDto
            {
                Id = index++,
                StationId = svc.StationId,
                EPGNumber = svc.EPGNumber,
                Color = color,
            });

        }

        return DataResponse<List<EPGColorDto>>.Success(epgColors);
    }
}
