namespace StreamMaster.Application.Statistics.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetInputStatisticsRequest() : IRequest<DataResponse<List<InputStreamingStatistics>>>;

internal class GetInputStatisticsHandler(IStreamStatisticService streamStatisticService)
    : IRequestHandler<GetInputStatisticsRequest, DataResponse<List<InputStreamingStatistics>>>
{
    public async Task<DataResponse<List<InputStreamingStatistics>>> Handle(GetInputStatisticsRequest request, CancellationToken cancellationToken)
    {
        var inputStatistics = streamStatisticService.GetInputStatistics();
        foreach (var inputStatistic in inputStatistics)
        {
            inputStatistic.UpdateValues();
        }
        return DataResponse<List<InputStreamingStatistics>>.Success(inputStatistics);

    }
}
