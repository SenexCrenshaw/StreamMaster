using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Dto;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager;

public class CircularRingBufferFactory(IStatisticsManager statisticsManager, IInputStatisticsManager inputStatisticsManager, IMemoryCache memoryCache, ILogger<ICircularRingBuffer> circularRingBufferLogger) : ICircularRingBufferFactory
{
    private readonly ConcurrentDictionary<string, ICircularRingBuffer> _circularRingBuffers = new();
    public ICircularRingBuffer CreateCircularRingBuffer(ChildVideoStreamDto childVideoStreamDto, int rank)
    {
        ICircularRingBuffer circularRingBuffer = new CircularRingBuffer(childVideoStreamDto, statisticsManager, inputStatisticsManager, memoryCache, rank, circularRingBufferLogger);
        _circularRingBuffers.TryAdd(childVideoStreamDto.User_Url, circularRingBuffer);
        return circularRingBuffer;
    }
    public ICircularRingBuffer? GetCircularRingBuffer(string StreamURL)
    {
        if (_circularRingBuffers.TryGetValue(StreamURL, out ICircularRingBuffer? circularRingBuffer))
        {
            return circularRingBuffer;
        }
        else
        {
            return null;
        }
    }
}
