namespace StreamMaster.Streams.Domain.Interfaces;


public interface IClientStreamerManager
{
    Task AddClientToHandler(Guid clientId, IStreamHandler streamHandler);
    Task AddClientsToHandler(int smChannelId, IStreamHandler streamHandler);

    List<IClientStreamerConfiguration> GetClientStreamerConfigurationsBySMChannelId(int smChannelId);

    int ClientCount(int ClientCount);

    Task<bool> CancelClient(Guid clientId, bool includeAbort);

    void Dispose();

    Task<IClientStreamerConfiguration?> GetClientStreamerConfiguration(Guid clientId, CancellationToken cancellationToken = default);


    ICollection<IClientStreamerConfiguration> GetAllClientStreamerConfigurations { get; }

    List<IClientStreamerConfiguration> GetClientStreamerConfigurationFromIds(List<Guid> clientIds);

    bool HasClient(string VideoStreamId, Guid ClientId);

    void RegisterClient(IClientStreamerConfiguration clientStreamerConfiguration);

    Task UnRegisterClient(Guid clientId);
}
