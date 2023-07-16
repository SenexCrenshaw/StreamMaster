using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces
{
    public interface IChannelManager
    {
        void Dispose();

        List<StreamStatisticsResult> GetAllStatisticsForAllUrls();

        SingleStreamStatisticsResult GetSingleStreamStatisticsResult(string streamUrl);

        Task<Stream?> GetStream(ClientStreamerConfiguration config);
        
        void RemoveClient(ClientStreamerConfiguration config);

        void SimulateStreamFailure(string streamUrl);

        void SimulateStreamFailureForAll();
        
    }
}
