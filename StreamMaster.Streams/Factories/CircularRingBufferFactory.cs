﻿using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.Infrastructure.VideoStreamManager.Buffers;

namespace StreamMaster.Streams.Factories;

public sealed class CircularRingBufferFactory(IInputStatisticsManager inputStatisticsManager, IMemoryCache memoryCache, ILoggerFactory loggerFactory) : ICircularRingBufferFactory
{
    public ICircularRingBuffer CreateCircularRingBuffer(VideoStreamDto videoStreamDto, string ChannelId, string ChannelName, int rank)
    {
        return new CircularRingBuffer(videoStreamDto, ChannelId, ChannelName, inputStatisticsManager, memoryCache, rank, loggerFactory);
    }
}
