using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Dto;

using StreamMasterInfrastructure.VideoStreamManager.Buffers;

namespace StreamMasterInfrastructure.VideoStreamManager.Factories;

public sealed class CircularRingBufferFactory(IStatisticsManager statisticsManager, IInputStatisticsManager inputStatisticsManager, IMemoryCache memoryCache, ILogger<ICircularRingBuffer> circularRingBufferLogger) : ICircularRingBufferFactory
{
    public ICircularRingBuffer CreateCircularRingBuffer(VideoStreamDto videoStreamDto, string ChannelName, int rank)
    {
        return new CircularRingBuffer(videoStreamDto, ChannelName, statisticsManager, inputStatisticsManager, memoryCache, rank, circularRingBufferLogger);
    }
}
