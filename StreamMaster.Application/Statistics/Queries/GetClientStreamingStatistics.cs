namespace StreamMaster.Application.Statistics.Queries;

public record GetClientStreamingStatistics() : IRequest<List<ClientStreamingStatistics>>;

internal class GetClientStreamingStatisticsHandler(IStreamStatisticService streamStatisticService) : IRequestHandler<GetClientStreamingStatistics, List<ClientStreamingStatistics>>
{
    public async Task<List<ClientStreamingStatistics>> Handle(GetClientStreamingStatistics request, CancellationToken cancellationToken)
    {
        return await streamStatisticService.GetClientStatistics(cancellationToken);
    }
}
