using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Statistics;

public sealed class ClientStatisticsManager(ILogger<ClientStatisticsManager> logger) : IStatisticsManager
{

    private readonly ConcurrentDictionary<Guid, ClientStreamingStatistics> _clientStatistics = new();
    private readonly ILogger<ClientStatisticsManager> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public void RegisterClient(IClientStreamerConfiguration streamerConfiguration)
    {
        _clientStatistics.TryAdd(streamerConfiguration.ClientId, new ClientStreamingStatistics(streamerConfiguration));
    }

    public void UnRegisterClient(Guid clientId)
    {

        _clientStatistics.TryRemove(clientId, out _);
    }

    public void AddBytesRead(Guid clientId, int count)
    {
        if (_clientStatistics.TryGetValue(clientId, out ClientStreamingStatistics? clientStats))
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
        return _clientStatistics.Values.ToList();
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
        if (_clientStatistics.TryGetValue(clientId, out ClientStreamingStatistics? clientStats))
        {
            clientStats.IncrementBytesRead();
        }
        else
        {
            _logger.LogWarning("Client {clientId} not found when trying to increment read.", clientId);
        }
    }
}