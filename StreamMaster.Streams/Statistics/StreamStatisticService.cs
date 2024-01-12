using Microsoft.Extensions.Caching.Memory;

using StreamMaster.Domain.Cache;

using System.Net;
using System.Net.Sockets;

namespace StreamMaster.Streams.Statistics;

public sealed class StreamStatisticService(IInputStatisticsManager inputStatisticsManager, IStatisticsManager statisticsManager, IMemoryCache memoryCache) : IStreamStatisticService
{
    public async Task<List<InputStreamingStatistics>> GetInputStatistics(CancellationToken cancellationToken = default)
    {

        return inputStatisticsManager.GetAllInputStreamStatistics();
    }

    public async Task<List<ClientStreamingStatistics>> GetClientStatistics(CancellationToken cancellationToken = default)
    {

        List<ClientStreamingStatistics> clientStreamingStatistics = statisticsManager.GetAllClientStatistics();
        Setting settings = memoryCache.GetSetting();

        if (settings.ShowClientHostNames)
        {
            foreach (ClientStreamingStatistics streamStatisticsResult in clientStreamingStatistics)
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

        return clientStreamingStatistics;
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
