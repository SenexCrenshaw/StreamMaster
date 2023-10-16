using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager;

public class ClientStreamerManager2(ILogger<ClientStreamerManager2> logger) : IClientStreamerManager2
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, ClientStreamerConfiguration>> _clientConfigurations = new();

    public int GetClientCount(string StreamURL)
    {
        IEnumerable<ClientStreamerConfiguration> configs = GetAllClientStreamerConfigurations(StreamURL);
        return configs.Count();
    }
    public void RegisterClientConfiguration(string StreamURL, ClientStreamerConfiguration config)
    {
        if (config == null)
        {
            logger.LogError("ClientStreamerConfiguration cannot be null");
            throw new ArgumentNullException(nameof(config));
        }

        AddClientConfiguration(StreamURL, config);
    }

    private void AddClientConfiguration(string clientType, ClientStreamerConfiguration configuration)
    {
        ConcurrentDictionary<Guid, ClientStreamerConfiguration> innerDict = _clientConfigurations.GetOrAdd(clientType, new ConcurrentDictionary<Guid, ClientStreamerConfiguration>());
        innerDict.AddOrUpdate(configuration.ClientId, configuration, (_, _) => configuration);
    }

    private bool RemoveClientConfiguration(string clientType, Guid clientId)
    {
        // Check if the outer dictionary contains the key
        if (_clientConfigurations.TryGetValue(clientType, out ConcurrentDictionary<Guid, ClientStreamerConfiguration>? innerDict))
        {
            // Try to remove the entry from the inner dictionary
            return innerDict.TryRemove(clientId, out _);
        }
        return false;
    }

    public bool UnregisterClientConfiguration(string StreamURL, Guid clientId)
    {
        if (!_clientConfigurations.ContainsKey(StreamURL))
        {
            return true;
        }
        return RemoveClientConfiguration(StreamURL, clientId);
    }

    public ClientStreamerConfiguration? GetClientStreamerConfiguration(string streamUrl, Guid clientId)
    {
        // Check if the outer dictionary contains the key for the StreamURL
        if (_clientConfigurations.TryGetValue(streamUrl, out ConcurrentDictionary<Guid, ClientStreamerConfiguration>? innerDict))
        {
            // Try to get the value from the inner dictionary using clientId
            if (innerDict.TryGetValue(clientId, out ClientStreamerConfiguration? clientConfig))
            {
                return clientConfig;
            }
        }

        // If we reach here, either the StreamURL was not in the outer dictionary,
        // or the clientId was not in the inner dictionary.
        return null;
    }

    public IEnumerable<ClientStreamerConfiguration> GetAllClientStreamerConfigurations(string streamUrl)
    {
        // Check if the outer dictionary contains the key for the StreamURL
        if (_clientConfigurations.TryGetValue(streamUrl, out ConcurrentDictionary<Guid, ClientStreamerConfiguration>? innerDict))
        {
            return innerDict.Values;
        }

        return Enumerable.Empty<ClientStreamerConfiguration>();
    }
}
