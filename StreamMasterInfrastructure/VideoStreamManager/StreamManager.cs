using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Dto;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager;

public class StreamManager(ILogger<CircularRingBuffer> circularBufferLogger, IClientStreamerManager clientStreamerManager, ILogger<StreamHandler> streamControllerlogger, ILogger<StreamManager> logger, IMemoryCache memoryCache) : IStreamManager
{

    private readonly ConcurrentDictionary<string, IStreamHandler> _streamControllers = new();


    public void Dispose()
    {
        // Dispose of each stream controller
        foreach (IStreamHandler streamController in _streamControllers.Values)
        {
            streamController.Dispose();
        }

        // Clear the dictionary
        _streamControllers.Clear();
    }

    public async Task<IStreamHandler?> GetOrCreateStreamController(ChildVideoStreamDto childVideoStreamDto, string videoStreamId, string videoStreamName, int rank)
    {
        string streamUrl = childVideoStreamDto.User_Url;

        if (!_streamControllers.TryGetValue(streamUrl, out IStreamHandler streamController))
        {
            streamController = await StreamHandler.CreateAsync(streamUrl, childVideoStreamDto, videoStreamId, videoStreamName, rank, streamControllerlogger, memoryCache, circularBufferLogger, clientStreamerManager);
            _streamControllers.TryAdd(streamUrl, streamController);
        }
        else
        {
            logger.LogInformation("Reusing buffer for stream: {StreamUrl}", streamUrl);
        }

        return streamController;
    }

    public SingleStreamStatisticsResult GetSingleStreamStatisticsResult(string streamUrl)
    {
        if (_streamControllers.TryGetValue(streamUrl, out IStreamHandler? _streamInformation))
        {
            return _streamInformation.RingBuffer.GetSingleStreamStatisticsResult();
        }
        return new SingleStreamStatisticsResult();
    }

    public IStreamHandler? GetStreamInformationFromStreamUrl(string streamUrl)
    {
        return _streamControllers.Values.FirstOrDefault(x => x.StreamUrl == streamUrl);
    }

    public ICollection<IStreamHandler> GetStreamInformations()
    {
        return _streamControllers.Values;
    }

    public int GetStreamsCountForM3UFile(int m3uFileId)
    {
        return _streamControllers.Count(x => x.Value.M3UFileId == m3uFileId);
    }
    public IStreamHandler? Stop(string streamUrl)
    {
        if (_streamControllers.TryRemove(streamUrl, out IStreamHandler? streamInformation))
        {
            streamInformation.Stop();
            return streamInformation;
        }
        logger.LogWarning("Failed to remove stream information for {StreamUrl}", streamUrl);
        return null;
    }
}
