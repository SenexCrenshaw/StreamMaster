using StreamMaster.Streams.Domain.Statistics;

namespace StreamMaster.Application.Statistics.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetStreamStreamingStatisticsRequest() : IRequest<DataResponse<List<StreamStreamingStatistic>>>;

internal class GetStreamStreamingStatisticsHandler(IStreamStatisticService streamStatisticService)
    : IRequestHandler<GetStreamStreamingStatisticsRequest, DataResponse<List<StreamStreamingStatistic>>>
{
    public async Task<DataResponse<List<StreamStreamingStatistic>>> Handle(GetStreamStreamingStatisticsRequest request, CancellationToken cancellationToken)
    {
        var inputStatistics = streamStatisticService.GetStreamStreamingStatistics();
        foreach (var inputStatistic in inputStatistics)
        {
            inputStatistic.UpdateValues();
        }
        return DataResponse<List<StreamStreamingStatistic>>.Success(inputStatistics);

    }
}
