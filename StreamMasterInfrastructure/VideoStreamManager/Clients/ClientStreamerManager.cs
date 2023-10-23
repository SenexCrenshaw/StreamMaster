using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager.Clients;

public class ClientStreamerManager(ILogger<ClientStreamerManager> logger) : IClientStreamerManager
{
    private readonly ConcurrentDictionary<Guid, IClientStreamerConfiguration> clientStreamerConfigurations = new();

    public void MoveClientStreamers(IStreamHandler oldStreamHandler, IStreamHandler newStreamHandler)
    {
        var oldConfigs = oldStreamHandler.GetClientStreamerConfigurations();

        if (oldConfigs == null)
        {
            return;
        }

        foreach (IClientStreamerConfiguration oldConfig in oldConfigs)
        {
            oldStreamHandler.UnRegisterClientStreamer(oldConfig);
            newStreamHandler.RegisterClientStreamer(oldConfig);
        }
    }

    public void Dispose()
    {
        clientStreamerConfigurations.Clear();
    }

    public int ClientCount(string ChannelVideoStreamId)
    {
        return clientStreamerConfigurations.Count(a => a.Value.ChannelVideoStreamId == ChannelVideoStreamId);
    }

    public void RegisterClient(IClientStreamerConfiguration clientStreamerConfiguration)
    {
        clientStreamerConfigurations.TryAdd(clientStreamerConfiguration.ClientId, clientStreamerConfiguration);
    }

    public void UnRegisterClient(Guid ClientId)
    {
        clientStreamerConfigurations.TryRemove(ClientId, out _);
    }

    public List<IClientStreamerConfiguration> GetClientStreamerConfigurationFromIds(List<Guid> clientIds)
    {
        return GetClientStreamerConfigurations.Where(a => clientIds.Contains(a.ClientId)).ToList();
    }

    public IClientStreamerConfiguration? GetClientStreamerConfiguration(Guid ClientId)
    {
        if (clientStreamerConfigurations.TryGetValue(ClientId, out IClientStreamerConfiguration? clientConfig))
        {
            return clientConfig;
        }
        logger.LogDebug("Client configuration for {ClientId} not found", ClientId);
        return null;
    }

    public bool CancelClient(Guid ClientId)
    {
        var config = GetClientStreamerConfiguration(ClientId);
        if (config == null) { return false; }

        logger.LogDebug("Cancelling {ClientId}", ClientId);
        config.ClientMasterToken.Cancel();
        return true;
    }

    public List<IClientStreamerConfiguration> GetClientStreamerConfigurationsByChannelVideoStreamId(string ChannelVideoStreamId)
    {
        var test = GetClientStreamerConfigurations.Where(a => a.ChannelVideoStreamId.Equals(ChannelVideoStreamId)).ToList();

        logger.LogDebug("Found {count} clients for ChannelVideoStreamId {ChannelVideoStreamId}", test.Count, ChannelVideoStreamId);

        return test;
    }

    public IClientStreamerConfiguration? GetClientStreamerConfiguration(string ChannelVideoStreamId, Guid ClientId)
    {
        var test = GetClientStreamerConfigurations.FirstOrDefault(a => a.ChannelVideoStreamId.Equals(ChannelVideoStreamId) && a.ClientId == ClientId);
        return test;
    }

    public void SetClientBufferDelegate(Guid ClientId, Func<ICircularRingBuffer> func)
    {
        IClientStreamerConfiguration? sc = GetClientStreamerConfiguration(ClientId);
        if (sc is null || sc.ReadBuffer is null)
        {
            return;
        }

        sc.ReadBuffer.SetBufferDelegate(func, sc);
    }

    public void FailClient(Guid clientId)
    {
        IClientStreamerConfiguration? c = GetClientStreamerConfiguration(clientId);

        if (c != null)
        {
            c.ClientMasterToken.Cancel();
            logger.LogWarning("Failed client: {clientId}", clientId);
        }
    }

    public List<IClientStreamerConfiguration> GetClientStreamerConfigurations => (List<IClientStreamerConfiguration>)clientStreamerConfigurations.Values;

    public bool HasClient(string ChannelVideoStreamId, Guid ClientId)
    {
        return GetClientStreamerConfiguration(ChannelVideoStreamId, ClientId) != null;
    }
}