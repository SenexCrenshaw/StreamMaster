namespace StreamMaster.Application.Statistics.Queries;

[SMAPI(NoDebug = true)]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetIsSystemReadyRequest : IRequest<DataResponse<bool>>;

internal class GetIsSystemReadyRequestHandler : IRequestHandler<GetIsSystemReadyRequest, DataResponse<bool>>
{
    public Task<DataResponse<bool>> Handle(GetIsSystemReadyRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(BuildInfo.IsSystemReady ? DataResponse.True : DataResponse.False);
    }
}
