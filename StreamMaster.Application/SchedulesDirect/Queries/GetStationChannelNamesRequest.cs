namespace StreamMaster.Application.SchedulesDirect.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetStationChannelNamesRequest : IRequest<DataResponse<List<StationChannelName>>>;

internal class GetStationChannelNamesHandler(ISchedulesDirectDataService schedulesDirectDataService)
    : IRequestHandler<GetStationChannelNamesRequest, DataResponse<List<StationChannelName>>>
{
    public Task<DataResponse<List<StationChannelName>>> Handle(GetStationChannelNamesRequest request, CancellationToken cancellationToken)
    {
        List<StationChannelName> channelNames = schedulesDirectDataService.GetStationChannelNames().ToList();

        //StationChannelName? dummy = channelNames.FirstOrDefault(a => a.ChannelName == "Dummy");
        //if (dummy != null)
        //{
        //    List<StationChannelName> list = channelNames.ToList();
        //    list.Remove(dummy);
        //    list.Insert(0, dummy);
        //    channelNames = list;
        //}

        return Task.FromResult<DataResponse<List<StationChannelName>>>(DataResponse<List<StationChannelName>>.Success(channelNames));
    }
}
