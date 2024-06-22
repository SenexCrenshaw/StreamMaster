namespace StreamMaster.Application.SchedulesDirect.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetChannelNamesRequest : IRequest<DataResponse<List<string>>>;

internal class GetChannelNamesRequestHandler(ISchedulesDirectDataService schedulesDirectDataService)
    : IRequestHandler<GetChannelNamesRequest, DataResponse<List<string>>>
{
    public async Task<DataResponse<List<string>>> Handle(GetChannelNamesRequest request, CancellationToken cancellationToken)
    {
        List<StationChannelName> channelNames = await schedulesDirectDataService.GetStationChannelNames();

        return DataResponse<List<string>>.Success(channelNames.OrderBy(a => a.DisplayName, StringComparer.OrdinalIgnoreCase).Select(a => a.ChannelName).ToList());
    }
}
