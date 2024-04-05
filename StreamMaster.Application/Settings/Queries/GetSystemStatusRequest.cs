namespace StreamMaster.Application.Settings.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetSystemStatusRequest : IRequest<APIResponse<SDSystemStatus>>;

internal class GetSystemStatusHandler : IRequestHandler<GetSystemStatusRequest, APIResponse<SDSystemStatus>>
{

    public Task<APIResponse<SDSystemStatus>> Handle(GetSystemStatusRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(APIResponse<SDSystemStatus>.Success(new SDSystemStatus { IsSystemReady = BuildInfo.SetIsSystemReady }));
    }
}
