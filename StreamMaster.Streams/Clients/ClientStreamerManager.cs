using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.Streams.Buffers;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Clients;

public sealed class ClientStreamerManager(ILogger<ClientStreamerManager> logger, IMemoryCache memoryCache, IStatisticsManager statisticsManager, ILogger<ClientReadStream> ringBufferReadStreamLogger) : IClientStreamerManager
{
    private readonly ConcurrentDictionary<Guid, IClientStreamerConfiguration> clientStreamerConfigurations = new();
    private readonly object _disposeLock = new();
    private bool _disposed = false;

    public void Dispose()
    {
        lock (_disposeLock)
        {
            if (_disposed)
            {
                return;
            }

            try
            {

                foreach (IClientStreamerConfiguration clientStreamerConfiguration in clientStreamerConfigurations.Values)
                {
                    CancelClient(clientStreamerConfiguration.ClientId, false).Wait();
                }
                clientStreamerConfigurations.Clear();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during disposing of StreamManager");
            }
            finally
            {
                _disposed = true;
            }
        }
    }

    public async Task AddClientsToHandler(string ChannelVideoStreamId, IStreamHandler streamHandler)
    {
        List<Guid> clientIds = GetClientStreamerConfigurationsByChannelVideoStreamId(ChannelVideoStreamId).ConvertAll(a => a.ClientId);
        foreach (Guid clientId in clientIds)
        {
            await AddClientToHandler(clientId, streamHandler).ConfigureAwait(false);
        }
    }

    public async Task AddClientToHandler(Guid clientId, IStreamHandler streamHandler)
    {
        IClientStreamerConfiguration? streamerConfiguration = await GetClientStreamerConfiguration(clientId);
        if (streamerConfiguration != null)
        {
            logger.LogDebug("Adding client {ClientId} {ReaderID} to {Circular Buffer ID} ", clientId, streamerConfiguration.ReadBuffer?.Id ?? Guid.NewGuid(), streamHandler.CircularRingBuffer.Id);
            streamHandler.RegisterClientStreamer(streamerConfiguration);
            streamerConfiguration.VideoStreamName = streamHandler.VideoStreamName;
            await SetClientBufferDelegate(streamerConfiguration, streamHandler.CircularRingBuffer);
        }
        else
        {
            logger.LogDebug("Error adding client {ClientId} to {Circular Buffer ID}, client status is null", clientId, streamHandler.CircularRingBuffer.Id);
        }
    }

    public int ClientCount(string ChannelVideoStreamId)
    {
        ConcurrentDictionary<Guid, IClientStreamerConfiguration> a = clientStreamerConfigurations;
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

    public async Task UnRegisterClient(Guid clientId)
    {
        await CancelClient(clientId, false).ConfigureAwait(false);

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

    public List<IClientStreamerConfiguration> GetClientStreamerConfigurationsByChannelVideoStreamId(string ChannelVideoStreamId)
    {
        ConcurrentDictionary<Guid, IClientStreamerConfiguration> a = clientStreamerConfigurations;
        List<IClientStreamerConfiguration> client = GetAllClientStreamerConfigurations.Where(a => a.ChannelVideoStreamId.Equals(ChannelVideoStreamId)).ToList();

        return client;
    }

    public IClientStreamerConfiguration? GetClientStreamerConfiguration(string ChannelVideoStreamId, Guid ClientId)
    {
        IClientStreamerConfiguration? test = GetAllClientStreamerConfigurations.FirstOrDefault(a => a.ChannelVideoStreamId.Equals(ChannelVideoStreamId) && a.ClientId == ClientId);
        return test;
    }

    public async Task SetClientBufferDelegate(IClientStreamerConfiguration clientStreamerConfiguration, ICircularRingBuffer RingBuffer)
    {
        clientStreamerConfiguration.ReadBuffer ??= new ClientReadStream(() => RingBuffer, memoryCache, statisticsManager, ringBufferReadStreamLogger, clientStreamerConfiguration);


        await clientStreamerConfiguration.ReadBuffer.SetBufferDelegate(() => RingBuffer, clientStreamerConfiguration);
    }

    public async Task<bool> CancelClient(Guid clientId, bool includeAbort)
    {
        IClientStreamerConfiguration? config = await GetClientStreamerConfiguration(clientId).ConfigureAwait(false);
        if (config == null) { return false; }

        logger.LogDebug("Cancelling client {ClientId}", clientId);

        //int test = GetAllClientStreamerConfigurations.Count(a => a.HttpContextId == config.HttpContextId);

        await config.CancelClient(includeAbort).ConfigureAwait(false);

        return true;
    }

    public ICollection<IClientStreamerConfiguration> GetAllClientStreamerConfigurations => clientStreamerConfigurations.Values;

    public bool HasClient(string ChannelVideoStreamId, Guid ClientId)
    {
        return GetClientStreamerConfiguration(ChannelVideoStreamId, ClientId) != null;
    }
}