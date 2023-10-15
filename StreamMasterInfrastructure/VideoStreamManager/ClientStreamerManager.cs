using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager;

public class ClientStreamerManager : IClientStreamerManager
{
    private readonly ConcurrentDictionary<Guid, ClientStreamerConfiguration> _clientConfigurations = new();

    public int ClientCount => _clientConfigurations.Count;

    public void RegisterClientConfiguration(ClientStreamerConfiguration config)
    {
        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        _clientConfigurations.TryAdd(config.ClientId, config);
    }

    public bool UnregisterClientConfiguration(Guid clientId)
    {
        return _clientConfigurations.TryRemove(clientId, out _);
    }

    public ClientStreamerConfiguration? GetClientConfiguration(Guid clientId)
    {
        _clientConfigurations.TryGetValue(clientId, out ClientStreamerConfiguration? config);
        return config;
    }

    public IEnumerable<ClientStreamerConfiguration> GetAllClientConfigurations()
    {
        return _clientConfigurations.Values;
    }
}
