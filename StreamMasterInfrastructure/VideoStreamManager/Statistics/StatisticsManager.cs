using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager.Statistics;

public class StatisticsManager(ILogger<StatisticsManager> logger) : IStatisticsManager
{
    private readonly ConcurrentDictionary<Guid, StreamingStatistics> _clientStatistics = new();
    private readonly ILogger<StatisticsManager> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public void RegisterClient(Guid clientId, string clientAgent, string clientIPAddress)
    {
        if (!_clientStatistics.ContainsKey(clientId))
        {
            _clientStatistics.TryAdd(clientId, new StreamingStatistics(clientAgent, clientIPAddress));
        }
    }

    public void UnregisterClient(Guid clientId)
    {
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
            _logger.LogWarning("Client {clientId} not found when trying to add bytes read.", clientId);
        }
    }
    public List<ClientStreamingStatistics> GetAllClientStatistics()
    {
        List<ClientStreamingStatistics> statisticsList = new();

        foreach (KeyValuePair<Guid, StreamingStatistics> entry in _clientStatistics)
        {
            statisticsList.Add(new ClientStreamingStatistics(entry.Value.ClientAgent, entry.Value.ClientIPAddress)
            {
                ClientId = entry.Key,
                BytesRead = entry.Value.BytesRead,
                StartTime = entry.Value.StartTime,
            });
        }

        return statisticsList;
    }

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