namespace StreamMaster.Application.Statistics.Queries;

public record GetInputStatistics() : IRequest<List<InputStreamingStatistics>>;

internal class GetInputStatisticsHandler(IStreamStatisticService streamStatisticService) : IRequestHandler<GetInputStatistics, List<InputStreamingStatistics>>
{
    public async Task<List<InputStreamingStatistics>> Handle(GetInputStatistics request, CancellationToken cancellationToken)
    {
        return await streamStatisticService.GetInputStatistics(cancellationToken);
    }
}
