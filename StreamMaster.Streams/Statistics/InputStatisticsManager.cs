using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Statistics;

public sealed class InputStatisticsManager(ILogger<InputStatisticsManager> logger) : IInputStatisticsManager
{
    private readonly ConcurrentDictionary<string, InputStreamingStatistics> _inputStatistics = new();
    private readonly ILogger<InputStatisticsManager> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public IInputStreamingStatistics RegisterInputReader(StreamInfo StreamInfo)
    {
        if (!_inputStatistics.ContainsKey(StreamInfo.VideoStreamId))
        {
            _inputStatistics.TryAdd(StreamInfo.VideoStreamId, new InputStreamingStatistics(StreamInfo));
        }

        return _inputStatistics[StreamInfo.VideoStreamId];
    }

    public IInputStreamingStatistics? GetInputStreamStatistics(string videoStreamId)
    {

        if (!_inputStatistics.TryGetValue(videoStreamId, out InputStreamingStatistics? _inputStreamStatistics))
        {
            _logger.LogWarning("Video stream {videoStreamId} not found when trying to get input stream statistics.", videoStreamId);
            return null;
        }

        return _inputStreamStatistics;
    }
    public List<InputStreamingStatistics> GetAllInputStreamStatistics()
    {
        foreach (KeyValuePair<string, InputStreamingStatistics> stat in _inputStatistics)
        {
            if (stat.Value.Clients == 0)
            {
                _inputStatistics.TryRemove(stat.Key, out _);
            }

        }

        return [.. _inputStatistics.Values];
    }
}