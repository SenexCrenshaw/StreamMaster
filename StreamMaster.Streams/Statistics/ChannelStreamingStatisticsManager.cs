using StreamMaster.Streams.Domain.Statistics;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Statistics;

public sealed class ChannelStreamingStatisticsManager(ILogger<ChannelStreamingStatisticsManager> logger, IStreamStreamingStatisticsManager streamStreamingStatisticsManager)
    : IChannelStreamingStatisticsManager
{
    private readonly ConcurrentDictionary<int, ChannelStreamingStatistics> _channelStreamingStatistics = new();
    private readonly ILogger<ChannelStreamingStatisticsManager> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public ChannelStreamingStatistics RegisterInputReader(SMChannelDto smChannelDto, int currentRank, string currentStreamId)
    {
        if (!_channelStreamingStatistics.TryGetValue(smChannelDto.Id, out ChannelStreamingStatistics channelStreamingStatistics))
        {
            channelStreamingStatistics = new ChannelStreamingStatistics();
            channelStreamingStatistics.SetStreamInfo(smChannelDto, currentRank, currentStreamId);
            _channelStreamingStatistics.TryAdd(smChannelDto.Id, channelStreamingStatistics);

        }

        channelStreamingStatistics.IncrementClient();
        return channelStreamingStatistics;
    }

    public bool UnRegister(int smChannelID)
    {
        return _channelStreamingStatistics.TryRemove(smChannelID, out _);
    }

    public void IncrementClient(int smChannelID)
    {
        if (_channelStreamingStatistics.TryGetValue(smChannelID, out ChannelStreamingStatistics? _inputStreamStatistics))
        {
            _inputStreamStatistics.IncrementClient();
        }
    }

    public void DecrementClient(int smChannelID)
    {
        if (_channelStreamingStatistics.TryGetValue(smChannelID, out ChannelStreamingStatistics? _inputStreamStatistics))
        {
            _inputStreamStatistics.DecrementClient();
            if (_inputStreamStatistics.Clients == 0)
            {
                _channelStreamingStatistics.TryRemove(smChannelID, out _);
            }
        }
    }

    public ChannelStreamingStatistics? GetChannelStreamingStatistic(int smChannelID)
    {

        if (!_channelStreamingStatistics.TryGetValue(smChannelID, out ChannelStreamingStatistics? _inputStreamStatistics))
        {
            _logger.LogWarning("Video stream {videoStreamId} not found when trying to get input stream statistics.", smChannelID);
            return null;
        }

        return _inputStreamStatistics;
    }
    public List<ChannelStreamingStatistics> GetChannelStreamingStatistics()
    {
        var streamStats = streamStreamingStatisticsManager.GetStreamingStatistics();
        foreach (KeyValuePair<int, ChannelStreamingStatistics> stat in _channelStreamingStatistics)
        {
            if (stat.Value.Clients == 0)
            {
                _channelStreamingStatistics.TryRemove(stat.Key, out _);
            }


        }


        return [.. _channelStreamingStatistics.Values];
    }
}