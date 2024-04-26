namespace StreamMaster.Application.SchedulesDirect.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetStationChannelNamesRequest : IRequest<DataResponse<List<StationChannelName>>>;

internal class GetStationChannelNamesHandler(ISchedulesDirectDataService schedulesDirectDataService)
    : IRequestHandler<GetStationChannelNamesRequest, DataResponse<List<StationChannelName>>>
{

    public async Task<DataResponse<List<StationChannelName>>> Handle(GetStationChannelNamesRequest request, CancellationToken cancellationToken)
    {
        List<StationChannelName> channelNames = await schedulesDirectDataService.GetStationChannelNames();
        return DataResponse<List<StationChannelName>>.Success(channelNames);

    }
}
