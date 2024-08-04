using StreamMaster.Domain.Events;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Handlers;

public class ChannelDistributorService(ILogger<ChannelDistributorService> logger, IVideoInfoService videoInfoService, ILogger<IChannelDistributor> channelDirectorlogger, IProxyFactory proxyFactory)
    : IChannelDistributorService
{
    public event AsyncEventHandler<ChannelDirectorStopped>? OnStoppedEvent;
    private readonly ConcurrentDictionary<string, IChannelDistributor> _channelDistributors = new();

    public IChannelDistributor? GetChannelDistributor(string? key)
    {
        return string.IsNullOrEmpty(key)
            ? null
            : !_channelDistributors.TryGetValue(key, out IChannelDistributor? channelDistributor) ? null : channelDistributor;
    }

    public List<IChannelDistributor> GetChannelDistributors()
    {
        return _channelDistributors.Values == null ? ([]) : ([.. _channelDistributors.Values]);
    }

    public IChannelDistributor? GetStreamHandler(string? key)
    {
        return string.IsNullOrEmpty(key)
            ? null
            : !_channelDistributors.TryGetValue(key, out IChannelDistributor? channelDistributor) ? null : channelDistributor;
    }
    public async Task<IChannelDistributor?> CreateChannelDistributorFromSMChannelDtoAsync(SMChannelDto smChannel, IChannelStatus channelStatus, CancellationToken cancellationToken)
    {

        await GetOrCreateSourceChannelDistributorSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_channelDistributors.TryGetValue(smChannel.Id.ToString(), out IChannelDistributor? channelDistributor))
            {
                if (channelDistributor.IsFailed)
                {
                    _ = StopAndUnRegister(smChannel.Id.ToString());

                }
                else
                {
                    logger.LogInformation("Reusing channel distributor: {Id} {name}", smChannel.Id, smChannel.Name);

                    return channelDistributor;
                }

            }


            channelDistributor = new ChannelDistributor(channelDirectorlogger, smChannel, channelStatus);

            if (channelDistributor == null)
            {
                logger.LogError("Could not create new channel distributor: {Id} {name}", smChannel.Id, smChannel.Name);
                return null;
            }
            logger.LogInformation("Created new channel distributor: {Id} {name}", smChannel.Id, smChannel.Name);

            channelDistributor.OnStoppedEvent += OnChannelStoppedEvent;
            bool test = _channelDistributors.TryAdd(smChannel.Id.ToString(), channelDistributor);

            return channelDistributor;
        }
        finally
        {
            GetOrCreateSourceChannelDistributorSlim.Release();
        }
    }

    private readonly SemaphoreSlim GetOrCreateSourceChannelDistributorSlim = new(1, 1);

    public async Task<IChannelDistributor?> GetOrCreateSourceChannelDistributorAsync(string channelName, SMStreamInfo smStreamInfo, CancellationToken cancellationToken)
    {
        await GetOrCreateSourceChannelDistributorSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_channelDistributors.TryGetValue(smStreamInfo.Url, out IChannelDistributor? channelDistributor))
            {
                if (channelDistributor.IsFailed)
                {
                    _ = StopAndUnRegister(smStreamInfo.Url);
                    _ = _channelDistributors.TryGetValue(smStreamInfo.Url, out channelDistributor);
                }

                logger.LogInformation("Reusing source channel distributor: {Id} {name}", smStreamInfo.Id, smStreamInfo.Name);
                return channelDistributor;
            }

            channelDistributor = new ChannelDistributor(channelDirectorlogger, smStreamInfo);

            if (channelDistributor == null)
            {
                logger.LogError("Could not create new source channel distributor: {Id} {name}", smStreamInfo.Id, smStreamInfo.Name);
                return null;
            }

            logger.LogInformation("Created new source channel distributor: {Id} {name}", smStreamInfo.Id, smStreamInfo.Name);

            (Stream? stream, int processId, ProxyStreamError? error) = await proxyFactory.GetProxy(smStreamInfo, cancellationToken).ConfigureAwait(false);
            if (stream == null || error != null || processId == 0)
            {
                logger.LogError("Could not source create stream for channel distributor: {Id} {name} {error}", smStreamInfo.Id, smStreamInfo.Name, error?.Message);
                return null;
            }
            channelDistributor.SetSourceStream(stream, channelName, smStreamInfo.Name, cancellationToken);

            channelDistributor.OnStoppedEvent += OnDistributorStoppedEvent;
            bool test = _channelDistributors.TryAdd(smStreamInfo.Url, channelDistributor);

            return channelDistributor;
        }
        finally
        {
            GetOrCreateSourceChannelDistributorSlim.Release();
        }
    }

    public IDictionary<string, IStreamHandlerMetrics> GetAggregatedMetrics()
    {
        Dictionary<string, IStreamHandlerMetrics> metrics = [];

        foreach (KeyValuePair<string, IChannelDistributor> kvp in _channelDistributors)
        {
            IChannelDistributor channelDistributor = kvp.Value;
            metrics[kvp.Key] = channelDistributor.GetMetrics;
        }

        return metrics;
    }
    private void OnChannelStoppedEvent(object? sender, ChannelDirectorStopped e)
    {
        OnStoppedEvent?.Invoke(sender!, e);
        StopAndUnRegister(e.SMStreamInfo.Id);

    }


    private void OnDistributorStoppedEvent(object? sender, ChannelDirectorStopped e)
    {
        OnStoppedEvent?.Invoke(sender!, e);
        StopAndUnRegister(e.SMStreamInfo.Url);
        videoInfoService.RemoveSourceChannel(e.SMStreamInfo.Name);
    }

    public bool StopAndUnRegister(string key)
    {
        if (key.Length == 0)
        {
            return false;
        }

        if (_channelDistributors.TryRemove(key, out IChannelDistributor? channelDistributor))
        {
            channelDistributor.Stop();
            return true;
        }

        //logger.LogWarning("Stop AndU nRegister channel distributor: {Id} doest not exist!", key);
        return false;
    }
}
