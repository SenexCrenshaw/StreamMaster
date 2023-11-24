using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Dto;

using StreamMasterInfrastructure.VideoStreamManager.Buffers;

namespace StreamMasterInfrastructure.VideoStreamManager.Factories;

public class CircularRingBufferFactory(IStatisticsManager statisticsManager, IInputStatisticsManager inputStatisticsManager, IMemoryCache memoryCache, ILogger<ICircularRingBuffer> circularRingBufferLogger) : ICircularRingBufferFactory
{
    public ICircularRingBuffer CreateCircularRingBuffer(VideoStreamDto videoStreamDto, int rank)
    {
        ICircularRingBuffer circularRingBuffer = new CircularRingBuffer(videoStreamDto, statisticsManager, inputStatisticsManager, memoryCache, rank, circularRingBufferLogger);
        return circularRingBuffer;
    }
}
