using StreamMaster.Streams.Domain.Metrics;

namespace StreamMaster.Application.Statistics.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetStreamConnectionMetricRequest(string Id) : IRequest<DataResponse<StreamConnectionMetric>>;

internal class GetStreamConnectionMetricRequestHandler(

    ISourceBroadcasterService sourceBroadcasterService)
    : IRequestHandler<GetStreamConnectionMetricRequest, DataResponse<StreamConnectionMetric>>
{
    public Task<DataResponse<StreamConnectionMetric>> Handle(GetStreamConnectionMetricRequest request, CancellationToken cancellationToken)
    {
        StreamConnectionMetric? metric = sourceBroadcasterService.GetStreamConnectionMetric(request.Id);
        return metric is null
            ? Task.FromResult(DataResponse<StreamConnectionMetric>.NotFound)
            : Task.FromResult(DataResponse<StreamConnectionMetric>.Success(metric));
    }
}
