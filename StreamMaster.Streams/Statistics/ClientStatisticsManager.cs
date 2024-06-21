using System.Collections.Concurrent;

namespace StreamMaster.Streams.Statistics;

public sealed class ClientStatisticsManager(ILogger<ClientStatisticsManager> logger) : IClientStatisticsManager
{
    private readonly ConcurrentDictionary<Guid, ClientStreamingStatistics> _clientStatistics = new();
    private readonly ILogger<ClientStatisticsManager> _logger = logger;

    public void RegisterClient(IClientStreamerConfiguration streamerConfiguration)
    {
        _clientStatistics.TryAdd(streamerConfiguration.ClientId, new ClientStreamingStatistics(streamerConfiguration));
    }

    public bool UnRegisterClient(Guid clientId)
    {
        return _clientStatistics.TryRemove(clientId, out _);
    }

    public void AddBytesRead(Guid clientId, int bytesRead)
    {
        if (_clientStatistics.TryGetValue(clientId, out ClientStreamingStatistics? clientStats))
        {
            clientStats.AddBytesRead(bytesRead);
        }

    }
    public List<ClientStreamingStatistics> GetAllClientStatistics()
    {
        return [.. _clientStatistics.Values];
    }

    public List<Guid> GetAllClientIds()
    {
        return [.. _clientStatistics.Keys];
    }

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