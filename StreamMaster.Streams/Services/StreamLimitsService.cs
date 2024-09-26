using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Enums;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Services;

public class StreamLimitsService(ILogger<StreamLimitsService> logger, ICacheManager CacheManager, IServiceProvider serviceProvider)
    : IStreamLimitsService
{
    public (int currentStreamCount, int maxStreamCount) GetStreamLimits(string smStreamId)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        SMStreamDto? smStreamDto = repositoryWrapper.SMStream.GetSMStream(smStreamId);
        if (smStreamDto != null)
        {
            (int currentStreamCount, int maxStreamCount) = GetStreamCountsForM3UFile(smStreamDto.M3UFileId);
            return (currentStreamCount, maxStreamCount);
        }
        return (0, 0);
    }

    public bool IsLimited(string smStreamId)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        SMStreamDto? smStreamDto = repositoryWrapper.SMStream.GetSMStream(smStreamId);
        return smStreamDto == null || IsLimited(smStreamDto);
    }

    private (int currentStreamCount, int maxStreamCount) GetStreamCountsForM3UFile(int M3UFileId)
    {
        ConcurrentDictionary<int, int> M3UStreamCount = new();
        List<IChannelBroadcaster> channelStatuses = CacheManager.ChannelBroadcasters.Values
               .Where(a => a.SMStreamInfo != null)
               .ToList();

        foreach (IChannelBroadcaster channelStatus in channelStatuses)
        {
            SMStreamDto? smStream = channelStatus.SMChannel.SMStreams.Find(a => a.Id == channelStatus.SMStreamInfo!.Id);

            if (smStream != null)
            {
                M3UStreamCount.AddOrUpdate(smStream.M3UFileId, 1, (_, oldValue) => oldValue + 1);
            }
        }

        int currentStreamCount = M3UStreamCount.GetValueOrDefault(M3UFileId, 0);
        int maxStreamCount = CacheManager.M3UMaxStreamCounts.GetValueOrDefault(M3UFileId, 0);

        return (currentStreamCount, maxStreamCount);
    }

    public bool IsLimited(SMStreamDto smStreamDto)
    {
        if (smStreamDto.SMStreamType > SMStreamTypeEnum.Regular || smStreamDto.M3UFileId < 0)
        {
            logger.LogInformation("Check stream limits for {name} : stream is custom, no limits", smStreamDto.Name);

            return false;
        }

        (int currentStreamCount, int maxStreamCount) = GetStreamCountsForM3UFile(smStreamDto.M3UFileId);

        logger.LogInformation("Check stream limits for {name} : currentStreamCount: {currentStreamCount}, maxStreamCount: {maxStreamCount}", smStreamDto.Name, currentStreamCount, maxStreamCount);
        return currentStreamCount >= maxStreamCount;
    }
}
