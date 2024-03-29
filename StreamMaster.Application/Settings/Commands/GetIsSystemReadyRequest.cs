namespace StreamMaster.Application.Settings.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetIsSystemReadyRequest : IRequest<bool>;

internal class GetIsSystemReadyRequestHandler : IRequestHandler<GetIsSystemReadyRequest, bool>
{
    public Task<bool> Handle(GetIsSystemReadyRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(BuildInfo.SetIsSystemReady);
    }
}
