using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Enums;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Services;

public class StreamLimitsService(ILogger<StreamLimitsService> logger, ICacheManager CacheManager, IServiceProvider serviceProvider)
    : IStreamLimitsService
{

    public bool IsLimited(string smStreamDtoId)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        SMStreamDto? smStreamDto = repositoryWrapper.SMStream.GetSMStream(smStreamDtoId);
        return smStreamDto == null || IsLimited(smStreamDto);
    }

    public bool IsLimited(SMStreamDto smStreamDto)
    {
        if (smStreamDto.SMStreamType > SMStreamTypeEnum.Regular || smStreamDto.M3UFileId < 0)
        {
            logger.LogInformation("Check stream limits for {name} : stream is custom, no limits", smStreamDto.Name);

            return false;
        }

        ConcurrentDictionary<int, int> M3UStreamCount = new();


        List<IChannelBroadcaster> channelStatuses = CacheManager.ChannelBroadcasters.Values
                .Where(a => a.SMStreamInfo != null)
                .ToList();


        foreach (IChannelBroadcaster channelStatus in channelStatuses)
        {
            SMStreamDto? smStream = channelStatus.SMChannel.SMStreams.Find(a => a.Id == channelStatus.SMStreamInfo!.Id);

            if (smStream != null)
            {

                // Get or initialize the maximum stream count for the M3U file
                //if (!CacheManager.M3UMaxStreamCounts.TryGetValue(smStream.M3UFileId, out int m3uLimit))
                //{
                //    M3UFile? m3uFile = await repositoryWrapper.M3UFile.GetM3UFileAsync(smStream.M3UFileId).ConfigureAwait(false);
                //    m3uLimit = (m3uFile?.MaxStreamCount) ?? 0;
                //    CacheManager.M3UMaxStreamCounts.TryAdd(smStream.M3UFileId, m3uLimit);
                //}

                // Increment the current stream count for the M3U file


                M3UStreamCount.AddOrUpdate(smStream.M3UFileId, 1, (_, oldValue) => oldValue + 1);

            }
        }

        // Check the limit for the specified SMStreamDto        
        int currentStreamCount = M3UStreamCount.GetValueOrDefault(smStreamDto.M3UFileId, 0);
        int maxStreamCount = CacheManager.M3UMaxStreamCounts.GetValueOrDefault(smStreamDto.M3UFileId, 0);
        logger.LogInformation("Check stream limits for {name} : currentStreamCount: {currentStreamCount}, maxStreamCount: {maxStreamCount}", smStreamDto.Name, currentStreamCount, maxStreamCount);
        return currentStreamCount >= maxStreamCount;
    }


}
