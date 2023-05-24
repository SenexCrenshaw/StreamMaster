using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces;

public interface IRingBufferManager
{
    void Dispose();

    SingleStreamStatisticsResult GetAllStatistics(string streamUrl);

    List<StreamStatisticsResult> GetAllStatisticsForAllUrls();

    (Stream? stream, Guid clientId, ProxyStreamError? error) GetStream(StreamerConfiguration config);

    void RemoveClient(StreamerConfiguration config);

    void SimulateStreamFailure(string streamUrl);

    // void StopStream(string streamUrl);
}
