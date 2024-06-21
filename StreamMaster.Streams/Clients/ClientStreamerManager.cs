using System.Collections.Concurrent;
namespace StreamMaster.Streams.Clients;

public sealed class ClientStreamerManager(ILogger<ClientStreamerManager> logger, IClientStatisticsManager clientStatisticsManager)
    : IClientStreamerManager
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


    public int ClientCount(int smChannelId)
    {
        ConcurrentDictionary<Guid, IClientStreamerConfiguration> a = clientStreamerConfigurations;
        return clientStreamerConfigurations.Count(a => a.Value.SMChannel.Id == smChannelId);
    }

    public bool RegisterClient(IClientStreamerConfiguration config)
    {
        if (!clientStreamerConfigurations.TryAdd(config.ClientId, config))
        {
            logger.LogWarning("Failed to register client: {ClientId}", config.ClientId);
            return false;
        }

        clientStatisticsManager.RegisterClient(config);
        return true;
    }

    public async Task UnRegisterClient(Guid clientId)
    {
        await CancelClient(clientId, false).ConfigureAwait(false);

        bool removed = clientStreamerConfigurations.TryRemove(clientId, out _);

        clientStatisticsManager.UnRegisterClient(clientId);
        if (!removed)
        {
            logger.LogWarning("Failed to unregister client: {ClientId}", clientId);
        }
    }


    public async Task CancelClient(Guid clientId, bool includeAbort = true)
    {
        IClientStreamerConfiguration? streamerConfiguration = await GetClientStreamerConfiguration(clientId);
        if (streamerConfiguration == null)
        {
            return;
        }

        if (streamerConfiguration.ClientStream != null)
        {
            streamerConfiguration.ClientStream.Channel?.Writer.Complete();
            streamerConfiguration.ClientStream.Cancel();
            streamerConfiguration.ClientStream.Dispose();
            streamerConfiguration.ClientStream = null;
        }

        try
        {
            if (includeAbort)
            {
                //if (!streamerConfiguration.Response.HasStarted)
                //{
                //    response.Body.Flush();

                //}
                //await response.CompleteAsync();
                //if (!response.HttpContext.Response.HasStarted)
                //{
                //    response.HttpContext.Abort();
                //}
            }
        }
        catch (ObjectDisposedException ex)
        {
            // Log the exception or handle it as necessary
        }
        catch (Exception ex)
        {

        }
        //ClientCancellationTokenSource.Cancel();

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

    //public List<IClientStreamerConfiguration> GetClientStreamerConfigurationFromIds(List<Guid> clientIds)
    //{
    //    return GetAllClientStreamerConfigurations.Where(a => clientIds.Contains(a.ClientId)).ToList();
    //}


    public List<IClientStreamerConfiguration> GetClientStreamerConfigurationsBySMChannelId(int smChannelId)
    {
        ConcurrentDictionary<Guid, IClientStreamerConfiguration> a = clientStreamerConfigurations;
        List<IClientStreamerConfiguration> client = GetAllClientStreamerConfigurations.Where(a => a.SMChannel.Id.Equals(smChannelId)).ToList();

        return client;
    }

    //public IClientStreamerConfiguration? GetClientStreamerConfiguration(string ChannelVideoStreamId, Guid ClientId)
    //{
    //    IClientStreamerConfiguration? test = GetAllClientStreamerConfigurations.FirstOrDefault(a => a.SMChannel.Id.Equals(ChannelVideoStreamId) && a.ClientId == ClientId);
    //    return test;
    //}



    public ICollection<IClientStreamerConfiguration> GetAllClientStreamerConfigurations => clientStreamerConfigurations.Values;

    //public bool HasClient(string ChannelVideoStreamId, Guid ClientId)
    //{
    //    return GetClientStreamerConfiguration(ChannelVideoStreamId, ClientId) != null;
    //}
}