using StreamMaster.Streams.Domain.Metrics;

namespace StreamMaster.Application.Statistics.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetStreamConnectionMetricsRequest() : IRequest<DataResponse<List<StreamConnectionMetric>>>;

internal class GetStreamConnectionMetricsRequestHandler(

    ISourceBroadcasterService sourceBroadcasterService)
    : IRequestHandler<GetStreamConnectionMetricsRequest, DataResponse<List<StreamConnectionMetric>>>
{
    public Task<DataResponse<List<StreamConnectionMetric>>> Handle(GetStreamConnectionMetricsRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(DataResponse<List<StreamConnectionMetric>>.Success(sourceBroadcasterService.GetStreamConnectionMetrics()));
    }
}
