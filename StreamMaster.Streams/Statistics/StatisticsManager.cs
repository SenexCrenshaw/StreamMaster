using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Statistics;

public sealed class StatisticsManager(ILogger<StatisticsManager> logger) : IStatisticsManager
{
    private readonly ConcurrentDictionary<Guid, IClientStreamerConfiguration> _clientConfigs = new();
    private readonly ConcurrentDictionary<Guid, StreamingStatistics> _clientStatistics = new();
    private readonly ILogger<StatisticsManager> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public void RegisterClient(IClientStreamerConfiguration streamerConfiguration)
    {
        _clientConfigs.TryAdd(streamerConfiguration.ClientId, streamerConfiguration);
        _clientStatistics.TryAdd(streamerConfiguration.ClientId, new StreamingStatistics(streamerConfiguration));
    }

    public void UnRegisterClient(Guid clientId)
    {
        _clientConfigs.TryRemove(clientId, out _);
        _clientStatistics.TryRemove(clientId, out _);
    }

    public void AddBytesRead(Guid clientId, int count)
    {
        if (_clientStatistics.TryGetValue(clientId, out StreamingStatistics? clientStats))
        {
            clientStats.AddBytesRead(count);
        }
        else
        {
            //_logger.LogWarning("Client {clientId} not found when trying to add bytes read.", clientId);
        }
    }
    public List<ClientStreamingStatistics> GetAllClientStatistics()
    {
        List<ClientStreamingStatistics> statisticsList = [];

        foreach (KeyValuePair<Guid, StreamingStatistics> entry in _clientStatistics)
        {

            IClientStreamerConfiguration? config = _clientConfigs[entry.Key];

            if (config != null)
            {
                statisticsList.Add(new ClientStreamingStatistics(config)
                {
                    ClientId = entry.Key,
                    BytesRead = entry.Value.BytesRead,
                    StartTime = entry.Value.StartTime,
                    ChannelName = entry.Value.ChannelName
                });
            }
        }

        return statisticsList;
    }

    public List<Guid> GetAllClientIds()
    {
        return [.. _clientStatistics.Keys];
    }

    //public List<ClientStreamingStatistics> GetAllClientStatisticsByClientIds(ICollection<Guid> ClientIds)
    //{
    //    List<ClientStreamingStatistics> statisticsList = [];

    //    foreach (KeyValuePair<Guid, StreamingStatistics> entry in _clientStatistics.Where(a => ClientIds.Contains(a.Key)))
    //    {

    //        //statisticsList.Add(new ClientStreamingStatistics(entry.Value.ClientAgent, entry.Value.ClientIPAddress)
    //        //{
    //        //    ClientId = entry.Key,
    //        //    BytesRead = entry.Value.BytesRead,
    //        //    StartTime = entry.Value.StartTime,
    //        //});
    //    }

    //    return statisticsList;
    //}

    public void IncrementBytesRead(Guid clientId)
    {
        if (_clientStatistics.TryGetValue(clientId, out StreamingStatistics? clientStats))
        {
            clientStats.IncrementBytesRead();
        }
        else
        {
            _logger.LogWarning("Client {clientId} not found when trying to increment read.", clientId);
        }
    }
}