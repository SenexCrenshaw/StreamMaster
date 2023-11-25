using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;

using StreamMasterInfrastructure.VideoStreamManager.Buffers;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager.Clients;

public class ClientStreamerManager(ILogger<ClientStreamerManager> logger, ILogger<RingBufferReadStream> ringBufferReadStreamLogger) : IClientStreamerManager
{
    private readonly ConcurrentDictionary<Guid, IClientStreamerConfiguration> clientStreamerConfigurations = new();

    public void MoveClientStreamers(IStreamHandler oldStreamHandler, IStreamHandler newStreamHandler)
    {
        IEnumerable<Guid> ClientIds = oldStreamHandler.GetClientStreamerClientIds();

        if (!ClientIds.Any())
        {
            return;
        }

        foreach (Guid clientId in ClientIds)
        {
            oldStreamHandler.UnRegisterClientStreamer(clientId);
            newStreamHandler.RegisterClientStreamer(clientId);
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
        bool added = clientStreamerConfigurations.TryAdd(clientStreamerConfiguration.ClientId, clientStreamerConfiguration);
        if (!added)
        {
            logger.LogWarning("Failed to register client: {ClientId}", clientStreamerConfiguration.ClientId);
            throw new InvalidOperationException($"Failed to register client: {clientStreamerConfiguration.ClientId}");
        }
    }

    public void UnRegisterClient(Guid clientId)
    {
        bool removed = clientStreamerConfigurations.TryRemove(clientId, out _);
        if (!removed)
        {
            logger.LogWarning("Failed to unregister client: {ClientId}", clientId);
        }

    }

    public List<IClientStreamerConfiguration> GetClientStreamerConfigurationFromIds(List<Guid> clientIds)
    {
        return GetAllClientStreamerConfigurations.Where(a => clientIds.Contains(a.ClientId)).ToList();
    }
    public async Task<IClientStreamerConfiguration?> GetClientStreamerConfiguration(Guid clientId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (clientStreamerConfigurations.TryGetValue(clientId, out IClientStreamerConfiguration? clientConfig))
        {
            return await Task.FromResult(clientConfig).ConfigureAwait(false);
        }
        logger.LogDebug("Client configuration for {ClientId} not found", clientId);
        return null;
    }

    public async Task<bool> CancelClient(Guid clientId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IClientStreamerConfiguration? config = await GetClientStreamerConfiguration(clientId, cancellationToken).ConfigureAwait(false);
        if (config == null) { return false; }

        logger.LogDebug("Cancelling {ClientId}", clientId);
        config.ClientMasterToken.Cancel();
        return true;
    }

    public List<IClientStreamerConfiguration> GetClientStreamerConfigurationsByChannelVideoStreamId(string ChannelVideoStreamId)
    {
        List<IClientStreamerConfiguration> client = GetAllClientStreamerConfigurations.Where(a => a.ChannelVideoStreamId.Equals(ChannelVideoStreamId)).ToList();

        return client;
    }

    public IClientStreamerConfiguration? GetClientStreamerConfiguration(string ChannelVideoStreamId, Guid ClientId)
    {
        IClientStreamerConfiguration? test = GetAllClientStreamerConfigurations.FirstOrDefault(a => a.ChannelVideoStreamId.Equals(ChannelVideoStreamId) && a.ClientId == ClientId);
        return test;
    }

    public async Task SetClientBufferDelegate(Guid clientId, ICircularRingBuffer RingBuffer, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IClientStreamerConfiguration? config = await GetClientStreamerConfiguration(clientId, cancellationToken).ConfigureAwait(false);
        if (config == null) { return; }

        if (config.ReadBuffer is null)
        {
            config.ReadBuffer = new RingBufferReadStream(() => RingBuffer, ringBufferReadStreamLogger, config);
        }

        config.ReadBuffer.SetBufferDelegate(() => RingBuffer, config);
    }

    public async Task FailClient(Guid clientId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IClientStreamerConfiguration? config = await GetClientStreamerConfiguration(clientId, cancellationToken).ConfigureAwait(false);
        if (config == null) { return; }

        if (config != null)
        {
            config.ClientMasterToken.Cancel();
            logger.LogWarning("Failed client: {clientId}", clientId);
        }
    }

    public ICollection<IClientStreamerConfiguration> GetAllClientStreamerConfigurations => clientStreamerConfigurations.Values;

    public bool HasClient(string ChannelVideoStreamId, Guid ClientId)
    {
        return GetClientStreamerConfiguration(ChannelVideoStreamId, ClientId) != null;
    }
}