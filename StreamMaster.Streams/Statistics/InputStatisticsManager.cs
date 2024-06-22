using System.Collections.Concurrent;

namespace StreamMaster.Streams.Statistics;

public sealed class InputStatisticsManager(ILogger<InputStatisticsManager> logger) : IInputStatisticsManager
{
    private readonly ConcurrentDictionary<string, InputStreamingStatistics> _inputStatistics = new();
    private readonly ILogger<InputStatisticsManager> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public IInputStreamingStatistics RegisterInputReader(StreamInfo StreamInfo)
    {
        if (!_inputStatistics.TryGetValue(StreamInfo.SMStream.Id, out InputStreamingStatistics streamInfo))
        {
            streamInfo = new InputStreamingStatistics();
            streamInfo.SetStreamInfo(StreamInfo);
            _inputStatistics.TryAdd(StreamInfo.SMStream.Id, streamInfo);

        }
        return streamInfo;
    }

    public bool UnRegister(string smStreamId)
    {
        return _inputStatistics.TryRemove(smStreamId, out _);
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