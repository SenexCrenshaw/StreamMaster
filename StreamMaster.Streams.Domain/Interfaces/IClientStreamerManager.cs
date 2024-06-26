namespace StreamMaster.Streams.Domain.Interfaces;


public interface IClientStreamerManager
{

    List<ClientStreamerConfiguration> GetClientStreamerConfigurationsBySMChannelId(int smChannelId);
    //ICollection<IClientStreamerConfiguration> GetAllClientStreamerConfigurations { get; }
    //List<IClientStreamerConfiguration> GetClientStreamerConfigurationFromIds(List<Guid> clientIds);
    //bool HasClient(string VideoStreamId, Guid ClientId);
    Task CancelClient(Guid clientId, bool includeAbort = true);
    int ClientCount(int ClientCount);

    //Task<bool> CancelClient(Guid clientId, bool includeAbort);

    void Dispose();

    Task<ClientStreamerConfiguration?> GetClientStreamerConfiguration(Guid clientId, CancellationToken cancellationToken = default);

    bool RegisterClient(ClientStreamerConfiguration config);

    Task UnRegisterClient(Guid clientId);


}
