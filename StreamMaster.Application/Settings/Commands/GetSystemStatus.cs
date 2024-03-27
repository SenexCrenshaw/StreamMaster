namespace StreamMaster.Application.Settings.Commands;

[SMAPI]
public record GetSystemStatus : IRequest<SDSystemStatus>;

internal class GetSystemStatusHandler : IRequestHandler<GetSystemStatus, SDSystemStatus>
{

    public Task<SDSystemStatus> Handle(GetSystemStatus request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SDSystemStatus { IsSystemReady = BuildInfo.SetIsSystemReady });
    }
}
