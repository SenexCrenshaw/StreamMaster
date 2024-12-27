namespace StreamMaster.Application.Statistics.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetStreamConnectionMetricDatasRequest() : IRequest<DataResponse<List<StreamConnectionMetricData>>>;

internal class GetStreamConnectionMetricDatasRequestHandler(

    ISourceBroadcasterService sourceBroadcasterService)
    : IRequestHandler<GetStreamConnectionMetricDatasRequest, DataResponse<List<StreamConnectionMetricData>>>
{
    public Task<DataResponse<List<StreamConnectionMetricData>>> Handle(GetStreamConnectionMetricDatasRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(DataResponse<List<StreamConnectionMetricData>>.Success(sourceBroadcasterService.GetStreamConnectionMetrics()));
    }
}
