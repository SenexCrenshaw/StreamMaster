using Microsoft.Extensions.Caching.Memory;

using StreamMaster.Application.Common.Interfaces;
using StreamMaster.Application.Common.Models;
using StreamMaster.Domain.Common;
using StreamMaster.Domain.Services;

using System.Net;
using System.Net.Sockets;

namespace StreamMaster.Infrastructure.VideoStreamManager.Statistics;

public sealed class StreamStatisticService(IStreamManager streamManager, ISettingsService settingsService, IMemoryCache memoryCache) : IStreamStatisticService
{

    public async Task<List<StreamStatisticsResult>> GetAllStatisticsForAllUrls(CancellationToken cancellationToken = default)
    {
        List<StreamStatisticsResult> allStatistics = [];

        List<IStreamHandler> infos = streamManager.GetStreamHandlers();
        foreach (IStreamHandler? info in infos.Where(a => a.CircularRingBuffer != null))
        {
            allStatistics.AddRange(info.CircularRingBuffer.GetAllStatisticsForAllUrls());
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
