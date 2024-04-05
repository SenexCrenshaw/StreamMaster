namespace StreamMaster.Application.Settings.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetIsSystemReadyRequest : IRequest<APIResponse<bool>>;

internal class GetIsSystemReadyRequestHandler : IRequestHandler<GetIsSystemReadyRequest, APIResponse<bool>>
{
    public Task<APIResponse<bool>> Handle(GetIsSystemReadyRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(BuildInfo.SetIsSystemReady ? APIResponse.True : APIResponse.False);
    }
}
