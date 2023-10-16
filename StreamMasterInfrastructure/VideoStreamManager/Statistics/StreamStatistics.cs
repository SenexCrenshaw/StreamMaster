using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Common;
using StreamMasterDomain.Services;

using System.Net;
using System.Net.Sockets;

namespace StreamMasterInfrastructure.VideoStreamManager.Statistics;

public class StreamStatisticService(IStreamManager streamManager, ISettingsService settingsService) : IStreamStatisticService
{

    public async Task<List<StreamStatisticsResult>> GetAllStatisticsForAllUrls()
    {
        List<StreamStatisticsResult> allStatistics = new();

        ICollection<IStreamHandler> infos = streamManager.GetStreamInformations();
        foreach (IStreamHandler? info in infos.Where(a => a.RingBuffer != null))
        {
            allStatistics.AddRange(info.RingBuffer.GetAllStatisticsForAllUrls());
        }

        Setting settings = await settingsService.GetSettingsAsync();

        if (settings.ShowClientHostNames)
        {
            foreach (StreamStatisticsResult streamStatisticsResult in allStatistics)
            {
                try
                {
                    IPHostEntry host = await Dns.GetHostEntryAsync(streamStatisticsResult.ClientIPAddress).ConfigureAwait(false);
                    streamStatisticsResult.ClientIPAddress = host.HostName;
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
}
