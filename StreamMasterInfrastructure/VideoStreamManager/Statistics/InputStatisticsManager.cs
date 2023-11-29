using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager.Statistics;

public sealed class InputStatisticsManager(ILogger<InputStatisticsManager> logger) : IInputStatisticsManager
{
    private readonly ConcurrentDictionary<string, IInputStreamingStatistics> _inputStatistics = new();
    private readonly ILogger<InputStatisticsManager> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public IInputStreamingStatistics RegisterReader(string videoStreamId)
    {
        if (!_inputStatistics.ContainsKey(videoStreamId))
        {
            _inputStatistics.TryAdd(videoStreamId, new InputStreamingStatistics());
        }

        return _inputStatistics[videoStreamId];
    }

    public IInputStreamingStatistics GetInputStreamStatistics(string videoStreamId)
    {
        if (!_inputStatistics.TryGetValue(videoStreamId, out IInputStreamingStatistics? _inputStreamStatistics))
        {
            _logger.LogWarning("Video stream {videoStreamId} not found when trying to get input stream statistics.", videoStreamId);
            return new InputStreamingStatistics();
        }

        return _inputStreamStatistics;
    }
}