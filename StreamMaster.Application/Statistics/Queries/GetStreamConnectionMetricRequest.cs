namespace StreamMaster.Application.Statistics.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetStreamConnectionMetricDataRequest(string Id) : IRequest<DataResponse<StreamConnectionMetricData>>;

internal class GetStreamConnectionMetricDataRequestHandler(

    ISourceBroadcasterService sourceBroadcasterService)
    : IRequestHandler<GetStreamConnectionMetricDataRequest, DataResponse<StreamConnectionMetricData>>
{
    public Task<DataResponse<StreamConnectionMetricData>> Handle(GetStreamConnectionMetricDataRequest request, CancellationToken cancellationToken)
    {
        StreamConnectionMetricData? metric = sourceBroadcasterService.GetStreamConnectionMetricData(request.Id);
        return metric is null
            ? Task.FromResult(DataResponse<StreamConnectionMetricData>.NotFound)
            : Task.FromResult(DataResponse<StreamConnectionMetricData>.Success(metric));
    }
}
