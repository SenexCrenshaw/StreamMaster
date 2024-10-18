namespace StreamMaster.Mdns;

public interface IAdvertiser : IDisposable
{
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
}
