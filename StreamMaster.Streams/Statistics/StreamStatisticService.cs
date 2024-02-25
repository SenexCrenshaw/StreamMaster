using Microsoft.Extensions.Caching.Memory;

using StreamMaster.Domain.Configuration;

using System.Net;
using System.Net.Sockets;

namespace StreamMaster.Streams.Statistics;

public sealed class StreamStatisticService(IInputStatisticsManager inputStatisticsManager, IMemoryCache memoryCache, IStatisticsManager statisticsManager, IOptionsMonitor<Setting> intsettings) : IStreamStatisticService
{
    private readonly Setting settings = intsettings.CurrentValue;

    public async Task<List<InputStreamingStatistics>> GetInputStatistics(CancellationToken cancellationToken = default)
    {

        return inputStatisticsManager.GetAllInputStreamStatistics();
    }

    public async Task<List<ClientStreamingStatistics>> GetClientStatistics(CancellationToken cancellationToken = default)
    {

        List<ClientStreamingStatistics> clientStreamingStatistics = statisticsManager.GetAllClientStatistics();

        if (settings.ShowClientHostNames)
        {
            foreach (ClientStreamingStatistics streamStatisticsResult in clientStreamingStatistics)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
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
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8),
        };

        memoryCache.Set(ipAddress, hostName, cacheEntryOptions);

        return hostName;
    }
}
