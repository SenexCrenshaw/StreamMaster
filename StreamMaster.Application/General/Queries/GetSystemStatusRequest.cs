namespace StreamMaster.Application.General.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetSystemStatusRequest : IRequest<DataResponse<SDSystemStatus>>;

internal class GetSystemStatusHandler : IRequestHandler<GetSystemStatusRequest, DataResponse<SDSystemStatus>>
{
    public Task<DataResponse<SDSystemStatus>> Handle(GetSystemStatusRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(DataResponse<SDSystemStatus>.Success(new SDSystemStatus { IsSystemReady = BuildInfo.IsSystemReady }));
    }
}
