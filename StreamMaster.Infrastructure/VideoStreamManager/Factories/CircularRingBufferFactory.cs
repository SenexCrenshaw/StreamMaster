using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Dto;

using StreamMaster.Application.Common.Interfaces;

using StreamMaster.Infrastructure.VideoStreamManager.Buffers;

namespace StreamMaster.Infrastructure.VideoStreamManager.Factories;

public sealed class CircularRingBufferFactory(IStatisticsManager statisticsManager, IInputStatisticsManager inputStatisticsManager, IMemoryCache memoryCache, ILogger<ICircularRingBuffer> circularRingBufferLogger) : ICircularRingBufferFactory
{
    public ICircularRingBuffer CreateCircularRingBuffer(VideoStreamDto videoStreamDto, string ChannelId, string ChannelName, int rank)
    {
        return new CircularRingBuffer(videoStreamDto, ChannelId, ChannelName, statisticsManager, inputStatisticsManager, memoryCache, rank, circularRingBufferLogger);
    }
}
