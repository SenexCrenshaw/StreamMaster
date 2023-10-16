using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Dto;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager;

public class StreamManager(ICircularRingBufferFactory circularRingBufferFactory, IProxyFactory proxyFactory, IClientStreamerManager clientStreamerManager, ILogger<StreamHandler> streamHandlerlogger, ILogger<StreamManager> logger, IMemoryCache memoryCache) : IStreamManager
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

    public async Task<IStreamHandler?> GetOrCreateStreamController(ChildVideoStreamDto childVideoStreamDto, int rank)
    {
        if (!_streamControllers.TryGetValue(childVideoStreamDto.User_Url, out IStreamHandler? streamController))
        {
            streamController = await StreamHandler.CreateAsync(streamHandlerlogger, childVideoStreamDto, proxyFactory, rank, circularRingBufferFactory, clientStreamerManager);
            if (streamController == null)
            {
                return null;
            }
            _streamControllers.TryAdd(childVideoStreamDto.User_Url, streamController);
        }
        else
        {
            logger.LogInformation("Reusing buffer for stream: {StreamUrl}", childVideoStreamDto.User_Url);
        }

        return streamController;
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
