using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamMasterInfrastructure.VideoStreamManager.Clients;


public class ClientStreamerManager(ILogger<ClientStreamerManager> logger) : IClientManager
{
    private readonly ConcurrentDictionary<Guid, ClientStreamerConfiguration> clientStreamerConfigurations = new();

    public void Dispose()
    {
        clientStreamerConfigurations.Clear();
        //GC.SuppressFinalize(this);
    }


    public int ClientCount => clientStreamerConfigurations.Count;

    public void RegisterClient(ClientStreamerConfiguration clientStreamerConfiguration)
    {
        clientStreamerConfigurations.TryAdd(clientStreamerConfiguration.ClientId, clientStreamerConfiguration);
    }
    public void UnRegisterClient(Guid clientId)
    {
        clientStreamerConfigurations.TryRemove(clientId, out _);
    }

    public List<ClientStreamerConfiguration> GetClientStreamerConfigurationFromIds(List<Guid> clientIds)
    {
        return GetClientStreamerConfigurations.Where(a => clientIds.Contains(a.ClientId)).ToList();
    }
    public ClientStreamerConfiguration? GetClientStreamerConfiguration(Guid clientID)
    {
        if (clientStreamerConfigurations.TryGetValue(clientID, out ClientStreamerConfiguration? clientConfig))
        {
            return clientConfig;
        }
        logger.LogDebug("Client configuration for {clientID} not found", clientID);
        return null;
    }

    public bool CancelClient(Guid clientID)
    {
        var config = GetClientStreamerConfiguration(clientID);
        if (    config == null) { return false; }

        logger.LogDebug("Cancelling {clientID}", clientID);
        config.ClientMasterToken.Cancel();
        return true;
    }


    public List<ClientStreamerConfiguration> GetClientStreamerConfigurationByVideoStreamId(string VideoStreamId)
    {
        var test = GetClientStreamerConfigurations.Where(a => a.VideoStreamId.Equals(VideoStreamId)).ToList();


        logger.LogDebug("Found {count} clients for VideoStreamId {VideoStreamId}", test.Count, VideoStreamId);

        return test;
    }


    public void SetClientBufferDelegate(Guid ClientId, Func<ICircularRingBuffer> func)
    {
        ClientStreamerConfiguration? sc = GetClientStreamerConfiguration(ClientId);
        if (sc is null || sc.ReadBuffer is null)
        {
            return;
        }

        sc.ReadBuffer.SetBufferDelegate(func, sc);
    }

    public List<ClientStreamerConfiguration> GetClientStreamerConfigurations => clientStreamerConfigurations.Values.ToList();


    public bool HasClient(Guid clientId)
    {
        return GetClientStreamerConfiguration(clientId) != null;
    }
}
