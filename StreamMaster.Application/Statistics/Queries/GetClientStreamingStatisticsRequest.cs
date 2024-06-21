namespace StreamMaster.Application.Statistics.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetClientStreamingStatisticsRequest() : IRequest<DataResponse<List<ClientStreamingStatistics>>>;

internal class GetClientStreamingStatisticsHandler(IStreamStatisticService streamStatisticService) : IRequestHandler<GetClientStreamingStatisticsRequest, DataResponse<List<ClientStreamingStatistics>>>
{
    public async Task<DataResponse<List<ClientStreamingStatistics>>> Handle(GetClientStreamingStatisticsRequest request, CancellationToken cancellationToken)
    {
        var clientStreamingStatistics = await streamStatisticService.GetClientStatistics();
        return DataResponse<List<ClientStreamingStatistics>>.Success(clientStreamingStatistics);

    }
}
