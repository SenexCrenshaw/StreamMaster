namespace StreamMaster.Application.Settings.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetSystemStatusRequest : IRequest<SDSystemStatus>;

internal class GetSystemStatusHandler : IRequestHandler<GetSystemStatusRequest, SDSystemStatus>
{

    public Task<SDSystemStatus> Handle(GetSystemStatusRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SDSystemStatus { IsSystemReady = BuildInfo.SetIsSystemReady });
    }
}
