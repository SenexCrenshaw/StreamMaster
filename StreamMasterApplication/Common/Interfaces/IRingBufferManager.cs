using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces;

public interface IRingBufferManager
{
    void Dispose();

    List<StreamStatisticsResult> GetAllStatisticsForAllUrls();

    SingleStreamStatisticsResult GetSingleStreamStatisticsResult(string streamUrl);

    Task<Stream?> GetStream(ClientStreamerConfiguration config);

    void RemoveClient(ClientStreamerConfiguration config);

    void SimulateStreamFailure(string streamUrl);

    void SimulateStreamFailureForAll();

    // void StopStream(string streamUrl);
}
