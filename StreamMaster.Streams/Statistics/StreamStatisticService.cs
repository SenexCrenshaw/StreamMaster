using Microsoft.Extensions.Caching.Memory;

using StreamMaster.Domain.Services;

using System.Net;
using System.Net.Sockets;

namespace StreamMaster.Streams.Statistics;

public sealed class StreamStatisticService(IStreamManager streamManager, IStatisticsManager statisticsManager, ISettingsService settingsService, IMemoryCache memoryCache) : IStreamStatisticService
{

    public async Task<List<StreamStatisticsResult>> GetAllStatisticsForAllUrls(CancellationToken cancellationToken = default)
    {
        List<StreamStatisticsResult> allStatistics = [];

        //List<IStreamHandler> infos = streamManager.GetStreamHandlers();
        //foreach (IStreamHandler? info in infos.Where(a => a.CircularRingBuffer != null))
        //{
        //    allStatistics.AddRange(info.GetAllStatisticsForAllUrls());
        //}

        foreach (ClientStreamingStatistics stat in statisticsManager.GetAllClientStatistics())
        {
            cancellationToken.ThrowIfCancellationRequested();
            allStatistics.Add(new StreamStatisticsResult
            {
                Id = Guid.NewGuid().ToString(),
                //CircularBufferId = Id.ToString(),
                //ChannelId = StreamInfo.ChannelId,
                //ChannelName = StreamInfo.ChannelName,
                //VideoStreamId = StreamInfo.VideoStreamId,
                //VideoStreamName = StreamInfo.VideoStreamName,
                //M3UStreamingProxyType = StreamInfo.StreamingProxyType,
                //Logo = StreamInfo.Logo,
                //Rank = StreamInfo.Rank,

                //InputBytesRead = input.BytesRead,
                //InputBytesWritten = input.BytesWritten,
                //InputBitsPerSecond = input.BitsPerSecond,
                //InputStartTime = input.StartTime,

                //StreamUrl = StreamInfo.StreamUrl,

                ClientBitsPerSecond = stat.ReadBitsPerSecond,
                ClientBytesRead = stat.BytesRead,
                ClientId = stat.ClientId,
                ClientStartTime = stat.StartTime,
                ClientAgent = stat.ClientAgent,
                ClientIPAddress = stat.ClientIPAddress
            });

        }

        Setting settings = await settingsService.GetSettingsAsync(cancellationToken);

        if (settings.ShowClientHostNames)
        {
            foreach (StreamStatisticsResult streamStatisticsResult in allStatistics)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    //IPHostEntry host = await Dns.GetHostEntryAsync(streamStatisticsResult.ClientIPAddress).ConfigureAwait(false);
                    string hostName = await GetHostNameAsync(streamStatisticsResult.ClientIPAddress, cancellationToken).ConfigureAwait(false);
                    streamStatisticsResult.ClientIPAddress = hostName;
                }
                catch (SocketException)
                {
                }
                catch (ArgumentException)
                {
                }
            }
        }

        return allStatistics;
    }

    private async Task<string> GetHostNameAsync(string ipAddress, CancellationToken cancellationToken)
    {
        if (memoryCache.TryGetValue(ipAddress, out string? hostName))
        {
            return hostName ?? "";
        }

        IPHostEntry host = await Dns.GetHostEntryAsync(ipAddress, cancellationToken).ConfigureAwait(false);
        hostName = host.HostName;

        // Set cache options. Adjust the expiration time as needed.
        MemoryCacheEntryOptions cacheEntryOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1), // e.g., 1 hour cache
            //SlidingExpiration = TimeSpan.FromMinutes(30) // e.g., extend cache life for another 30 minutes if accessed
        };

        memoryCache.Set(ipAddress, hostName, cacheEntryOptions);

        return hostName;
    }
}
